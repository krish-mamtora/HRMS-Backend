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
        IEmployeeCycleStatsService _employeeCycleStatsService;
        public BookingService(MyDbContext context, IMapper mapper, IFairnessService fairnessService, IGameSlotService gameSlotService, IUserProfileService userProfileService, IEmployeeCycleStatsService employeeCycleStatsService )
        {
            _context = context;
            _mapper = mapper;
            _fairnessService = fairnessService;
            _gameSlotService = gameSlotService;
            _userProfileService = userProfileService;
            _employeeCycleStatsService = employeeCycleStatsService;
        }
        public async Task<BookingResultDto> RequestBookingAsync(BookingRequestCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var result = new BookingResultDto
                {
                    UserResults = new List<UserBookingDetail>(),
                    BookedUsers = new List<int>(),
                    WaitingUsers = new List<int>()
                };

                var slot = await _context.GameSlots
                    .Include(s => s.GameCycle)
                    .FirstOrDefaultAsync(s => s.Id == dto.SlotId);

                if (slot == null || !slot.IsBookingOpen)
                    throw new Exception("Slot unavailable");

                int availableSeats = slot.Capacity - slot.Assigned;

                var eligibleForBooking = new List<int>();
                var queueUsers = new List<int>();

                foreach (var userId in dto.userIds.Distinct())
                {
                  
                    var (rejected, message) =
                        await _fairnessService.IsHardRejectedAsync(userId, dto.SlotId);

                    if (rejected)
                    {
                        result.UserResults.Add(new UserBookingDetail
                        {
                            UserId = userId,
                            Status = "Failed",
                            Message = message
                        });
                        continue;
                    }

                    bool eligible =
                        await _fairnessService.IsEligibleForDirectBookingAsync(userId, slot.CycleId);

                    if (eligible && availableSeats > 0)
                    {
                        eligibleForBooking.Add(userId);
                        availableSeats--;

                        result.UserResults.Add(new UserBookingDetail
                        {
                            UserId = userId,
                            Status = "Booked",
                            Message = "Slot booked successfully"
                        });
                    }
                    else
                    {
                        queueUsers.Add(userId);

                        result.UserResults.Add(new UserBookingDetail
                        {
                            UserId = userId,
                            Status = "Waiting",
                            Message = eligible
                                ? "Slot full. Added to queue"
                                : "Not eligible now. Added to queue"
                        });
                    }
                }

                if (eligibleForBooking.Any())
                {
                    int bookedBy = eligibleForBooking.Contains(dto.BookedBy)
                        ? dto.BookedBy
                        : eligibleForBooking.First();

                    var booking = new Bookings
                    {
                        SlotId = dto.SlotId,
                        BookedBy = bookedBy,
                        Status = "Booked",
                        BookedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _context.Bookings.AddAsync(booking);
                    await _context.SaveChangesAsync();

                    foreach (var userId in eligibleForBooking)
                    {
                        await _context.BookingParticipants.AddAsync(
                            new BookingParticipants
                            {
                                BookingId = booking.BId,
                                EmpId = userId
                            });
                    }

                    slot.Assigned += eligibleForBooking.Count;
                    result.BookedUsers.AddRange(eligibleForBooking);
                }

               
                foreach (var userId in queueUsers)
                {
                    bool exists = await _context.WaitingQueue.AnyAsync(q =>
                        q.PlayerId == userId &&
                        q.SlotId == dto.SlotId &&
                        q.Status == "Waiting");

                    if (!exists)
                    {
                        await _context.WaitingQueue.AddAsync(new WaitingQueue
                        {
                            PlayerId = userId,
                            SlotId = dto.SlotId,
                            CycleId = slot.CycleId,
                            Status = "Waiting",
                            InsertionTime = DateTime.UtcNow
                        });
                    }

                    result.WaitingUsers.Add(userId);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task CancelBookingAsync(int bookingId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.GameSlots)
                    .Include(b => b.BookingParticipants)
                    .FirstOrDefaultAsync(b => b.BId == bookingId);

                if (booking == null)
                    throw new Exception("Booking not found");

                if (booking.Status != "Booked")
                    throw new Exception("Only booked slots can be cancelled");

                var slot = booking.GameSlots;

              
                booking.Status = "Cancelled";
                booking.UpdatedAt = DateTime.UtcNow;

              
                slot.Assigned -= booking.BookingParticipants.Count;
                if (slot.Assigned < 0)
                    slot.Assigned = 0;

             
                foreach (var participant in booking.BookingParticipants)
                {
                    await _employeeCycleStatsService
                        .DecreaseGamePlayedAsync(participant.EmpId, slot.CycleId);
                }

                await _context.SaveChangesAsync();

                await PromoteFromWaitingQueueAsync(slot.Id);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        private async Task PromoteFromWaitingQueueAsync(int slotId)
        {
            var slot = await _context.GameSlots
                .FirstOrDefaultAsync(s => s.Id == slotId);

            if (slot == null)
                return;

            int availableSeats = slot.Capacity - slot.Assigned;

            if (availableSeats <= 0)
                return;

            var waitingList = await _context.WaitingQueue
                .Where(q => q.SlotId == slotId && q.Status == "Waiting")
                .ToListAsync();

            if (!waitingList.Any())
                return;

            var priorityList = new List<(WaitingQueue Queue, int Priority)>();

            foreach (var q in waitingList)
            {
                var (isRejected, _) = await _fairnessService
                    .IsHardRejectedAsync(q.PlayerId, slotId);

                if (isRejected)
                {
                    q.Status = "Removed";
                    continue;
                }

                int priority = await _fairnessService
                    .GetUserPriorityAsync(q.PlayerId, slot.CycleId);

                priorityList.Add((q, priority));
            }

            if (!priorityList.Any())
                return;

            var orderedQueue = priorityList
                .OrderBy(x => x.Priority)            
                .ThenBy(x => x.Queue.InsertionTime)   
                .ToList();

            var usersToPromote = orderedQueue
                .Take(availableSeats)
                .ToList();

            if (!usersToPromote.Any())
                return;

            var newBooking = new Bookings
            {
                SlotId = slotId,
                BookedBy = usersToPromote.First().Queue.PlayerId,
                Status = "Booked",
                BookedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Bookings.AddAsync(newBooking);
            await _context.SaveChangesAsync();

            foreach (var item in usersToPromote)
            {
                await _context.BookingParticipants.AddAsync(new BookingParticipants
                {
                    BookingId = newBooking.BId,
                    EmpId = item.Queue.PlayerId
                });

                await _employeeCycleStatsService
                    .IncreaseGamePlayedAsync(item.Queue.PlayerId, slot.CycleId);

                item.Queue.Status = "Promoted";
            }

            slot.Assigned += usersToPromote.Count;

            await _context.SaveChangesAsync();
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
