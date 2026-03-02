using AutoMapper;
using HRMS_Backend.Data;
using HRMS_Backend.Entities;
using HRMS_Backend.Entities.Games_Scheduling;
using HRMS_Backend.Entities.GamesScheduling;
using HRMS_Backend.Model.GameScheduling;
using HRMS_Backend.Services.Email;
using HRMS_Backend.Services.ServiceUserProfile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;

namespace HRMS_Backend.Services.GameScheduling
{
    public class BookingService : IBookingService
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
       private readonly IEmailService _emailService;
        IFairnessService _fairnessService;
        IGameSlotService _gameSlotService;
        IUserProfileService _userProfileService;
        IEmployeeCycleStatsService _employeeCycleStatsService;
        public BookingService(MyDbContext context, IMapper mapper, IFairnessService fairnessService, IGameSlotService gameSlotService, IUserProfileService userProfileService, IEmployeeCycleStatsService employeeCycleStatsService , IEmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _fairnessService = fairnessService;
            _gameSlotService = gameSlotService;
            _userProfileService = userProfileService;
            _employeeCycleStatsService = employeeCycleStatsService;
            _emailService = emailService;
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
                  
                    var (rejected, message) = await _fairnessService.IsHardRejectedAsync(userId, dto.SlotId);

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
                    int bookedBy = eligibleForBooking.Contains(dto.BookedBy) ? dto.BookedBy:eligibleForBooking.First();

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
                if (result.BookedUsers.Any())
                {
                    await NotifyBookedUsersAsync(result.BookedUsers, slot, dto.BookedBy);
                }
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
        private async Task PromoteFromWaitingQueueAsync(int slotId, bool sendEmail = true)
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
                .Take(availableSeats)
                .ToList();

            if (!orderedQueue.Any())
                return;

            var newBooking = new Bookings
            {
                SlotId = slotId,
                BookedBy = orderedQueue.First().Queue.PlayerId,
                Status = "Booked",
                BookedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Bookings.AddAsync(newBooking);
            await _context.SaveChangesAsync();

            var promotedUserIds = new List<int>();

            foreach (var item in orderedQueue)
            {
                await _context.BookingParticipants.AddAsync(new BookingParticipants
                {
                    BookingId = newBooking.BId,
                    EmpId = item.Queue.PlayerId
                });

                item.Queue.Status = "Promoted";
                promotedUserIds.Add(item.Queue.PlayerId);
            }

            slot.Assigned += promotedUserIds.Count;

            await _context.SaveChangesAsync();

            if (sendEmail && promotedUserIds.Any())
            {           
                await NotifyBookedUsersAsync(promotedUserIds, slot , newBooking.BookedBy);
            }
        }
        public async Task<BookingsDisplayDto> getBookingById(int id)
        {
            var booking = await _context.Bookings.Include(b=>b.BookingParticipants).FirstOrDefaultAsync(b=>b.BId == id);
            return _mapper.Map<BookingsDisplayDto>(booking);
        }
        public async Task<IEnumerable<BookingsDisplayDto>> getBookingsByUserId(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid User ID", nameof(id));

            var bookingIds = await _context.BookingParticipants
                .Where(bp => bp.EmpId == id)
                .Select(bp => bp.BookingId)
                .Distinct()
                .ToListAsync();

            if (!bookingIds.Any()) return new List<BookingsDisplayDto>();

            var bookings = await _context.Bookings
                .Include(b => b.BookingParticipants) 
                .Where(b => bookingIds.Contains(b.BId))
                .ToListAsync();

            return _mapper.Map<IEnumerable<BookingsDisplayDto>>(bookings);
        }

        private async Task NotifyBookedUsersAsync(List<int> userIds, GameSlots slot, int requesterId) 
        {
            var requester = await _context.Users.FindAsync(requesterId);
            string requesterEmail = requester?.Email ?? "A Colleague";

            var usersToNotify = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Email })
                .ToListAsync();

            foreach (var user in usersToNotify)
            {
                if (string.IsNullOrEmpty(user.Email)) continue;

                try
                {
                    var subject = "Success: Your Game Slot is Booked!";

                    var body = $@"
                Slot Booking Confirmation
                
                Hello,

                Great news! Your slot has been successfully booked.

                --- Booking Details ---
                Slot: {slot.Bookings}
                Date: {slot.StartTime:dddd, dd MMMM yyyy}
                Time: {slot.StartTime:hh:mm tt} - {slot.EndTime:hh:mm tt}
                Booked By: {requesterEmail}

                --- Location/Instructions ---
                Please ensure you arrive 5 minutes before the start time.

                Date of Request: {DateTime.UtcNow:f} UTC

                If you cannot attend, please cancel your booking to allow others to play.";

                    await _emailService.SendEmailAsync(user.Email, subject, body);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending booking email to {user.Email}: {ex.Message}");
                }
            }
        }
        public async Task MarkSlotCompletedAsync(int slotId, int completedByUserId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            Console.WriteLine("MMMMMMMMMMM");
            Console.WriteLine($"{slotId} {completedByUserId}");
            try
            {
                var slot = await _context.GameSlots.Include(s => s.Bookings).ThenInclude(b => b.BookingParticipants).FirstOrDefaultAsync(s => s.Id == slotId);
                if (slot == null)
                    throw new Exception("Slot not found");

                if (slot.SlotPlayed)
                {
                    throw new Exception("Slot already completed");
                } 
                if (DateTime.Now < slot.StartTime) { 
                    throw new Exception("Slot has not started yet");
                }

                var activeBooking = slot.Bookings.FirstOrDefault(b => b.Status == "Booked");

                if (activeBooking == null)
                { 
                    throw new Exception("No active booking found"); 
                }

                activeBooking.Status = "Completed";
                activeBooking.UpdatedAt = DateTime.UtcNow;

                var participantIds = activeBooking.BookingParticipants.Select(p => p.EmpId).ToList();


                var userStats = await _context.EmployeeCycleStats
                   .Where(es => es.UserId == completedByUserId)
                   .OrderByDescending(es => es.GameCycleId) 
                   .FirstOrDefaultAsync();

                if (userStats != null)
                {
                    userStats.GamePlayed++;
                    Console.WriteLine("++++++++++++++++");
                }

                slot.Assigned -= activeBooking.BookingParticipants.Count;
                if (slot.Assigned < 0)
                    slot.Assigned = 0;

                var remainingMinutes = (slot.EndTime - DateTime.UtcNow).TotalMinutes;
                int minimumPlayableMinutes = 15;

                if (remainingMinutes >= minimumPlayableMinutes)
                {
                    await PromoteFromWaitingQueueAsync(slotId, true);
                }
                else
                {
                    var waitingUsers = await _context.WaitingQueue
                        .Where(q => q.SlotId == slotId && q.Status == "Waiting")
                        .ToListAsync();

                    var expiredUserIds = waitingUsers
                        .Select(q => q.PlayerId)
                        .ToList();

                    foreach (var q in waitingUsers)
                        q.Status = "Expired";

                    if (expiredUserIds.Any())
                        await NotifyBookedUsersAsync(expiredUserIds, slot , activeBooking.User.Id);

                    slot.SlotPlayed = true;
                    slot.IsBookingOpen = false;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

    }
}

                //Console.WriteLine($"{participantIds}");
                //if (participantIds.Any())
                //{
                //    await _employeeCycleStatsService.IncrementCompletedPlayCountAsync(participantIds, slot.CycleId);
                //}
                //Console.WriteLine("^^^^^^^^^^^^^");
                //foreach (var participant in activeBooking.BookingParticipants)
                //{
                //    await _employeeCycleStatsService
                //        .IncreaseGamePlayedAsync(participant.EmpId, slot.CycleId);
                //}

                //Console.WriteLine("++++++++++++++++");