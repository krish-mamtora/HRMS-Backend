using AutoMapper;
using HRMS_Backend.Data;

using HRMS_Backend.Services.ServiceUserProfile;
using Microsoft.EntityFrameworkCore;
using static HRMS_Backend.Services.GameScheduling.BookingService;

namespace HRMS_Backend.Services.GameScheduling
{
    public class FairnessService : IFairnessService
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        private readonly IGameCycleService _gameCycleService;
        private readonly IEmployeeCycleStatsService _employeeCycleStatsService;
        private readonly IUserProfileService _userProfileService;
      
        public FairnessService(MyDbContext context, IMapper mapper , IGameCycleService gameCycleService, IEmployeeCycleStatsService employeeCycleStatsService ,IUserProfileService userProfileService)
        {
            _context = context;
            _mapper = mapper;
            _gameCycleService = gameCycleService;
            _employeeCycleStatsService = employeeCycleStatsService;
            _userProfileService = userProfileService;
        
        }

        public async Task<Boolean> IsUsersEligibleAsync(int slotId, int userId , int cycleId)
        {
            var slotGameName = await _context.GameSlots.Where(s=>s.Id == slotId).Select(s=>s.Games.Name).FirstOrDefaultAsync();

            if(slotGameName == null)
            {
                return false;
            }
            var interestedGame = await _userProfileService.GetGameInterestedByIdAsync(userId);

            if (interestedGame != slotGameName)
            {
                return false;
            }
            var todayStart = DateTime.UtcNow.Date;
            var tommorowStart = todayStart.AddDays(1);

            Boolean playedToday = await _context.BookingParticipants.Where(p=>p.EmpId == userId && p.Bookings.SlotPlayed == true)
                .AnyAsync(p=>p.Bookings.GameSlots.StartTime >= todayStart && p.Bookings.GameSlots.StartTime < tommorowStart);
            //Console.WriteLine($"Played TOday state : {playedToday}");

            if (playedToday) {
                return false;
            }

            var lowestGamePlayed = await _gameCycleService.getLowsetGamePlayedInCurrentCycle(cycleId);
            //Console.WriteLine($"CHECK lowest state : {lowestGamePlayed}");

            var stats = await _employeeCycleStatsService.GetUserCycleStatsAsync(userId, cycleId);

            int userPlayed = (stats != null) ? (stats.GamePlayed != null ? stats.GamePlayed : 0) : 0;
 
            //Console.WriteLine($"CHECK EMP cycle state : {userPlayed}");
            if (userPlayed > lowestGamePlayed) {
                return false;
            }
            return true;
        }
       
    }

}