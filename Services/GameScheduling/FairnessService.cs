using AutoMapper;
using HRMS_Backend.Data;
using HRMS_Backend.Entities;
using HRMS_Backend.Entities.Games_Scheduling;
using HRMS_Backend.Model.GameScheduling;
using HRMS_Backend.Services.ServiceUserProfile;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Boolean> IsUsersEligibleAsync(int slotId, int userId , int cycleId , int gameId)
        {          
            return await CheckAllValidations(slotId , userId , cycleId  , gameId);
        }
        
        public async Task<string> IsUsersEligibAsync(int slotId, int userId, int cycleId, int gameId)
        {
             if(CheckUserIsInterestedInGame(userId, gameId))
            {
                if (await !CheckSlotCapacity(slotId) ||await CheckUserHasPlaydToday(userId) ||await  !ComparePlayedGameValueInCycle(userId, cycleId))
                {
                    return "Waiting-Queue";
                }
                else if (await CheckSlotCapacity(slotId) && await !CheckUserHasPlaydToday(userId) && await ComparePlayedGameValueInCycle(userId, cycleId))

                {
                    return "Booking";
                }
            }
        }
   
        public async Task<Boolean> CheckAllValidations(int slotId , int userId,int cycleId, int gameId)
        {
            return (CheckUserIsInterestedInGame(userId,gameId)
                && ComparePlayedGameValueInCycle( userId,  cycleId)
                && (CheckSlotCapasityAndUserWIthTeamCount(userId,  cycleId)
                && (CheckTeamMembersValidation()
                && (CheckUserHasPlaydToday(int userId))
                )

        }

        public async Task<Boolean> CheckUserIsInterestedInGame (int userId , int gameId)
        {
            var userSport = await _userProfileService.GetGameInterestedByIdAsync(userId);
            var gameName = await _context.Games.Where(g => g.Id == gameId).Select(g => g.Name).FirstOrDefaultAsync();
            return userSport == gameName;
        }

        public async Task<Boolean> ComparePlayedGameValueInCycle(int userId , int cycleId)
        {
           var lowsetGamePlayed = await _gameCycleService.getLowsetGamePlayedInCurrentCycle(cycleId);
            var userPlayedSlots = await _employeeCycleStatsService.GetUserCycleStatsAsync(userId, cycleId);
        
            return (userPlayedSlots.GamePlayed > lowsetGamePlayed) ? false : true;
        }
        public async Task<Boolean> CheckSlotCapacity(int slotId)
        {

        }
        public async Task<Boolean> CheckSlotCapasityAndUserWIthTeamCount(int userId, int cycleId)
        {

        }
        public async Task<Boolean> CheckTeamMembersValidation()
        {

        }
        public async Task<Boolean> CheckUserHasPlaydToday(int userId, int cycleId)
        {
            //move back to waiting queue
        }
        public async Task<Boolean> IncrementCompletedPlayCountAsync(int userId,int  cycleId)
        {

        }

      
    }
}
