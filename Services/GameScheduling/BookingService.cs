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

namespace HRMS_Backend.Services.GameScheduling
{
    public class BookingService : IBookingService
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        IFairnessService _fairnessService;
        IGameSlotService _gameSlotService;
        IUserProfileService _userProfileService;
        public BookingService(MyDbContext context, IMapper mapper, IFairnessService fairnessService , IGameSlotService gameSlotService, IUserProfileService userProfileService)
        {
            _context = context;
            _mapper = mapper;
            _fairnessService = fairnessService;
            _gameSlotService = gameSlotService;
            _userProfileService = userProfileService;
        }
        public async Task<BookingResultDto> RequestBookingAsync(BookingRequestCreateDto dto)
        {
             if(dto==null){
                throw new ArgumentNullException(nameof(dto));
            }
            return await ManageRequestBookingAsync(dto.SlotId, dto.userIds, dto.BookedBy);
        }
        public async Task<BookingResultDto> ManageRequestBookingAsync(int slotId, List<int> userIds, int bookedBy)
        {
            Console.WriteLine($"KKKK slotId: {slotId}");
            Console.WriteLine($"bookedBy: {bookedBy}");
            Console.WriteLine($"userIds: {(userIds != null ? string.Join(",", userIds) : "null")}");

            //await Task.Delay(0);
            if (userIds == null || userIds.Count == 0)
            {
                    throw new ArgumentException("User list cannot be empty", nameof(userIds)); 
            }
            var transection = await _context.Database.BeginTransactionAsync();
            var result = new BookingResultDto
            {     
                BookedUsers = new List<int>(),
                WaitingUsers = new List<int>()
            };
            //return result;
            var slot = await _context.GameSlots.FirstOrDefaultAsync(s=>s.Id== slotId);
            if (slot == null)
            {
                throw new Exception("Invalid Slot");
            }
            int availableSeats = slot.Capacity - slot.Assigned;
            var eligibleUsers = new List<int>();
            var waitingUsers = new List<int>();
            Console.WriteLine($"LLL avaialeseat: {availableSeats}");
            //Console.WriteLine($"bookedBy: {bookedBy}");
            //Console.WriteLine($"userIds: {(userIds != null ? string.Join(",", userIds) : "null")}");

            foreach (var userId in userIds.Distinct())
            {
                if(await _userProfileService.IsUserBannedAsync(userId))
                {
                    Console.WriteLine($"CCCCCCCCCCCCCCCCCCC");
                    continue;
                }
                Boolean alreadyBooked = await _context.BookingParticipants.AnyAsync(p=>p.EmpId== userId && p.Bookings.SlotId == slotId && p.Bookings.Status == "Booked");
                if (alreadyBooked) {
                    Console.WriteLine($"BBBBBBBBBBB");
                    continue;
                }
                bool isEligible = await _fairnessService.IsUsersEligibleAsync(slotId, userId, slot.CycleId);
                if(isEligible && slot.IsBookingOpen && availableSeats > 0)
                {
                    Console.WriteLine($"DDDDDDDDD");
                    eligibleUsers.Add(userId);
                    availableSeats--;
                }
                else
                {
                    Console.WriteLine($"QQQQQQQQQQQQQQQQ");
                    waitingUsers.Add(userId);
                }
            }

            Bookings booking = null;

            if (eligibleUsers.Any())
            {
                Console.WriteLine($"YYYEEESSSSSSSSSSS");

                booking = new Bookings
                {
                    SlotId = slotId,
                    BookedBy = bookedBy,
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
                slot.AvailableSeats  -= eligibleUsers.Count;
                result.BookedUsers.AddRange(eligibleUsers);
            }


            //if (waitingUsers.Any())
            //{
            //    Console.WriteLine($"NOOOOOOOOOO");
            //    foreach (var userId in waitingUsers)
            //    {
            //        await _context.WaitingQueue.AddAsync(new WaitingQueue
            //        {
            //            PlayerId = userId,
            //            SlotId = slotId,
            //            CycleId = slot.CycleId,
            //            Status = "Waiting",
            //            InsertionTime = DateTime.Now,
            //        });
            //        //result.WaitingUsers.Add(userId);
            //    }
            //    result.WaitingUsers.AddRange(waitingUsers);
            //    await _context.SaveChangesAsync();
            //    await transection.CommitAsync();
            //}
            if (waitingUsers?.Any() == true)
            {
                Console.WriteLine($"QUEUEQUEUEQUEUE");
                var entities = waitingUsers.Select(userId => new WaitingQueue
                {
                    PlayerId = userId,
                    SlotId = slotId,
                    CycleId = slot.CycleId,
                    Status = "Waiting",
                    InsertionTime = DateTime.UtcNow
                }).ToList();

                await _context.WaitingQueue.AddRangeAsync(entities);
                Console.WriteLine($"Inserted in queue");
                result.WaitingUsers ??= new List<int>();
                result.WaitingUsers.AddRange(waitingUsers);

                await _context.SaveChangesAsync();
            }

            return result;
        }

        public async Task<Boolean> CancelBooking(int bookingId)
        {
            var transection = await _context.Database.BeginTransactionAsync();
            var booking = await _context.Bookings.Include(b => b.BookingParticipants).FirstOrDefaultAsync(b => b.BId == bookingId);

            if (booking != null)
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
                if (eligible)
                {
                    await _context.BookingParticipants.AddAsync(
                        new BookingParticipants
                        {
                            BookingId = bookingId,
                            EmpId = user.PlayerId

                        });

                    user.Status = "Promoted";
                    slot.Assigned++;
                    relaesedSeats--;
                }

            }
            await _context.SaveChangesAsync();
            await transection.CommitAsync();
            return true;
        }
        public async Task<BookingsDisplayDto> getBookingById(int id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            var booking = await _context.Bookings.FindAsync(id);
            return _mapper.Map<BookingsDisplayDto>(booking);
        }

    }
}
