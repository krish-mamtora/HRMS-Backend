using AutoMapper;
using HRMS_Backend.Data;
using HRMS_Backend.Entities.Games_Scheduling;
using HRMS_Backend.Entities.GamesScheduling;
using HRMS_Backend.Model.GameScheduling;
using HRMS_Backend.Model.JobListing;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Backend.Services.GameScheduling
{
    public class GameCycleService : IGameCycleService
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        public GameCycleService(MyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IEnumerable<GameCycle>> GetAllGameCyclesAsync()
        {
            return await _context.GameCycle.ToListAsync();
        }


        public async Task<GameCycle> GetGameCycleByIdAsync(int id)
        {
            return await _context.GameCycle.FirstOrDefaultAsync(gc => gc.CycleId == id);
        }
        public async Task<GameCycleDisplayDto> CreateGameCycleAsync(GameCycleCreateUpdateDto dto)
        {
            
            var gameCycle = new GameCycle
            {
                GamesId = dto.GamesId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                isActive = dto.isActive
            };
            _context.GameCycle.Add(gameCycle);
            await _context.SaveChangesAsync();

            var game = await _context.Games.FindAsync(dto.GamesId);
            var interestedEmployee = await _context.UserProfile.Where(up => up.FavouriteSport == game.Name).Select(ep => ep.UserProfileId).ToListAsync();

            Console.Write(interestedEmployee);
            Console.Write("list is here");
            if (interestedEmployee.Count > 0)
            {


                 await InitializeCycleStatsAsyc(gameCycle.CycleId, interestedEmployee);
                 await _context.SaveChangesAsync();
                 return _mapper.Map<GameCycleDisplayDto>(gameCycle);
            }
            else
            {
                throw new Exception("Failed to create Cycle");
            }
            
        }
       
        public async Task<GameCycleDisplayDto> getCycleById(int id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            var gameCycle = await _context.GameCycle.FindAsync(id);
            var GameCycleDto = _mapper.Map<GameCycleDisplayDto>(gameCycle);
            return GameCycleDto;
        }

        public async Task<int> GetActiveCycleIdAsync(int gameId)
        {
            if (gameId <= 0)
            {
                throw new ArgumentException("Game Id must be greter then 0", nameof(gameId));
            }
            var activeCycleId = await _context.GameCycle.Where(gc => gc.GamesId == gameId && gc.isActive)
                    .Select(gc => gc.CycleId).FirstOrDefaultAsync();

            return activeCycleId;
        }
        public async Task<int> getLowsetGamePlayedInCurrentCycle(int cycleId)
        {
            var lowestGamePlayedVal = await _context.EmployeeCycleStats
             .Where(es => es.GameCycleId == cycleId)
             .MinAsync(e => Convert.ToInt32(e.GamePlayed));
            return lowestGamePlayedVal;
        }
        public async Task<Boolean> InitializeCycleStatsAsyc(int cycleId, List<int> InteretedUser)
        {
            var stats = InteretedUser.Select(userId => new EmployeeCycleStats
            {
                UserId = userId,
                GameCycleId = cycleId,
                GamePlayed = 0
            }).ToList();

            await _context.EmployeeCycleStats.AddRangeAsync(stats);

            await _context.SaveChangesAsync();
            return true;
        }


    }
}
