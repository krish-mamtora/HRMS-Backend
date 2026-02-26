using AutoMapper;
using HRMS_Backend.Data;
using HRMS_Backend.Entities;
using HRMS_Backend.Entities.Games_Scheduling;
using HRMS_Backend.Entities.GamesScheduling;
using HRMS_Backend.Model.GameScheduling;
using HRMS_Backend.Services.ServiceUserProfile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;

namespace HRMS_Backend.Services.GameScheduling
{
    public class BookingService : IBookingService
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        IFairnessService _fairnessService;
        IGameSlotService _gameSlotService;
        IUserProfileService _userProfileService;
        public BookingService(MyDbContext context, IMapper mapper, IFairnessService fairnessService, IGameSlotService gameSlotService, IUserProfileService userProfileService)
        {
            _context = context;
            _mapper = mapper;
            _fairnessService = fairnessService;
            _gameSlotService = gameSlotService;
            _userProfileService = userProfileService;
        }
        public async Task<BookingResultDto> RequestBookingAsync(BookingRequestCreateDto dto)
        {
            if (dto == null) {
                throw new ArgumentNullException(nameof(dto));
            }
            if (dto.userIds == null || !dto.userIds.Any()) {
                throw new ArgumentException("User list cannot be empty");
            }
            var transection = await _context.Database.BeginTransactionAsync();
            try
            {
                var result = new BookingResultDto
                {
                    UserResults  = new List<UserBookingDetail>(),
                    BookedUsers = new List<int>(),
                    WaitingUsers = new List<int>()
                };
                var slot = await _context.GameSlots.FirstOrDefaultAsync(s=>s.Id== dto.SlotId);
                if (slot == null)
                {
                    throw new Exception("Invalid Slot");
                }
                int availableSeats = slot.Capacity - slot.Assigned;
                var eligibleUsers = new List<int>();
                var waitingUsers = new List<int>();

                foreach (var userId in dto.userIds.Distinct())
                {
                    if(await _userProfileService.IsUserBannedAsync(userId))
                    {
                        result.UserResults.Add(new UserBookingDetail { UserId = userId, Status = "Failed", Message = "User is banned" });
                        continue;
                    }
                    Boolean alreadyBooked = await _context.BookingParticipants.AnyAsync(p=>p.EmpId== userId && p.Bookings.SlotId == dto.SlotId && p.Bookings.Status == "Booked");
                    if (alreadyBooked) {
                        result.UserResults.Add(new UserBookingDetail { UserId = userId, Status = "Failed", Message = "User already booked for this slot" });
                        continue;
                    }
                    bool isEligible = await _fairnessService.IsUsersEligibleAsync(dto.SlotId, userId, slot.CycleId);
                    if(isEligible && slot.IsBookingOpen && availableSeats > 0)
                    {
                        result.UserResults.Add(new UserBookingDetail { UserId = userId, Status = "Booked", Message = "Successfully booked" });
                        eligibleUsers.Add(userId);
                        availableSeats--;
                    }
                    else
                    {
                        result.UserResults.Add(new UserBookingDetail { UserId = userId, Status = "Waiting", Message = "Slot full/closed, added to waiting queue" });
                        waitingUsers.Add(userId);
                    }
                }

            if (eligibleUsers.Any())
            {
                var booking = new Bookings
                {
                    SlotId = dto.SlotId,
                    BookedBy = dto.BookedBy,
                    SlotPlayed = false,
                    Status = "Booked",
                };
                await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync();


                foreach (var userId in eligibleUsers)
                {
                    await _context.BookingParticipants.AddAsync(
                        new BookingParticipants
                        {
                            BookingId = booking.BId,
                            EmpId = userId
                        }
                    );
                }
                slot.Assigned += eligibleUsers.Count;
                result.BookedUsers.AddRange(eligibleUsers);
            }
                foreach (var userId in waitingUsers)
                {
                    Boolean alreadyInQueue = await _context.WaitingQueue.AnyAsync(q => q.PlayerId == userId && q.SlotId == dto.SlotId && q.Status == "Waiting");
                    if (!alreadyInQueue)
                    {
                        await _context.WaitingQueue.AddAsync(new WaitingQueue
                        {
                            PlayerId = userId,
                            SlotId = dto.SlotId,
                            CycleId = slot.CycleId,
                            Status = "Waiting",
                            InsertionTime = DateTime.UtcNow,
                        });
                    }
                    result.WaitingUsers.Add(userId);
                }
                  await _context.SaveChangesAsync();
                await transection.CommitAsync();

                return result;

            }
            catch
            {
                await transection.RollbackAsync();
                throw;
            }
        
        }

        public async Task<Boolean> CancelBooking(int bookingId)
        {
            var transection = await _context.Database.BeginTransactionAsync();
            try
            {
                var booking = await _context.Bookings.Include(b => b.BookingParticipants).FirstOrDefaultAsync(b => b.BId == bookingId);

                if (booking == null)
                {
                    return false;
                }
                booking.Status = "Cancelled";

                var slot = await _context.GameSlots.FirstAsync(s => s.Id == booking.SlotId);
                int relaesedSeats = booking.BookingParticipants.Count();

                slot.Assigned -= relaesedSeats;

                var queueUsers = await _context.WaitingQueue.Where(q => q.SlotId == slot.Id && q.Status == "Waiting").OrderBy(q => q.InsertionTime).ToListAsync();
              
                foreach (var user in queueUsers)
                {
                    if (relaesedSeats == 0)
                    {
                        break;
                    }
                    Boolean eligible = await _fairnessService.IsUsersEligibleAsync(slot.Id, user.PlayerId, slot.CycleId);
                    
                    if (!eligible)
                    {
                        continue;
                    }
                    var newBooking = new Bookings
                    {
                        SlotId = slot.Id,
                        BookedBy = user.PlayerId,
                        SlotPlayed = false,
                        Status = "Booked",
                    };

                    await _context.Bookings.AddAsync(newBooking);

                    await _context.SaveChangesAsync();

                    await _context.BookingParticipants.AddAsync(
                        new BookingParticipants
                        {
                            BookingId = newBooking.BId,
                            EmpId = user.PlayerId,
                        });

                    user.Status = "Promoted";
                    slot.Assigned++;
                    relaesedSeats--;
                }
                await _context.SaveChangesAsync();
                await transection.CommitAsync();

                return true;
            }
            catch
            {
                await transection.RollbackAsync();
                throw;
            }
        }
        public async Task<BookingsDisplayDto> getBookingById(int id)
        {
            var booking = await _context.Bookings.Include(b=>b.BookingParticipants).FirstOrDefaultAsync(b=>b.BId == id);
            return _mapper.Map<BookingsDisplayDto>(booking);
        }
        public async Task<IEnumerable<BookingsDisplayDto>> getBookingsByUserId(int id)
        {
            if (id<=0)
            {
                throw new ArgumentNullException(nameof(id));
            }
            var bookings = await _context.BookingParticipants.Where(bk => bk.EmpId==id).Select(bp=>bp.BookingId).Distinct().ToListAsync();
            return _mapper.Map<IEnumerable<BookingsDisplayDto>>(bookings);
        }

    }
}
