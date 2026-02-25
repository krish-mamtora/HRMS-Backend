using AutoMapper;
using HRMS_Backend.Data;
using HRMS_Backend.Entities;
using HRMS_Backend.Entities.Games_Scheduling;
using HRMS_Backend.Entities.GamesScheduling;
using HRMS_Backend.Model.GameScheduling;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace HRMS_Backend.Services.GameScheduling
{
    public class BookingService : IBookingService
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        IFairnessService _fairnessService;
        IGameSlotService _gameSlotService;
        public BookingService(MyDbContext context, IMapper mapper, IFairnessService fairnessService , IGameSlotService gameSlotService)
        {
            _context = context;
            _mapper = mapper;
            _fairnessService = fairnessService;
            _gameSlotService = gameSlotService;
        }

        public async Task<BookingsCreateUpdateDto> RequestBookingAsync(int slotId, List<int> userIds , int bookedBy , int gameId)
        {
            //var slotBook = _gameSlotService.GetSlotStatus(slotId);
            var cycleId;
            Boolean slotbooked = false;
            foreach(var user in userIds)
            {
                var actoin = await _fairnessService.IsUsersEligibAsync(slotId, userId, cycleId, gameId);
                if (action == "Waiting-Queue")
                {

                }
                else if (action == "Booking")
                {
                    if (!slotbooked)
                    {
                        try
                        {
                            var booking = await BookSlot(slotId, bookedBy);

                        }
                        catch (Exception ex) { 
                        
                        }
                    }
                    try
                    {
                        UpdateEmployeeCyclestats(user, cycleId);

                        BookingParticipants(user, bookingId);
                    }
                    catch (Exception ex) { 
                        
                    }
                }

            }
       
        }
        public async Task<Bookings> BookSlot(int slotId, int bookedBy)
        {
            var booking = new Bookings
            {
                SlotId = slotId,
                BookedBy = bookedBy,
                SlotPlayed = false,//get from gameslot , bookby through
                Status = "Booked"
            };
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
            return booking;
        }
        
        public async Task<BookingsDisplayDto> IsUserAlreadyBookedAsync(slotId, userId)
        {

        }
        public async Task<> UpdateEmployeeCyclestats(int userId , int cycleId)
        {

        }
        public async Task<> CancleBooking(int BookingId) {
            UpdateStatusOfTimeSlot();
            UpdateDailyLimit();
            IdentifyNextInQueue();
            AssignSlotToNextPerson();
            RemoveNewlyAssignedmemeberFromQueue();
            bothEmail();

        }
        public async Task<> check()
        {
            /// if user dont reach withing 10 minute and not even cancell , then cancel their booking and 
            /// /allocat to next eligible perso from queue 
        }
        public async Task<> SlotEnds(int BookingId)
        {
            updateCycleGamePlayedCount();
            TriggerQueueforNextAllocation();
            moveallofThemtoBackofQueue();
        }
    }
}
