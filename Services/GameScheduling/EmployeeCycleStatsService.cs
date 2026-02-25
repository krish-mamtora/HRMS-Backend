using AutoMapper;
using HRMS_Backend.Data;
using HRMS_Backend.Entities.GamesScheduling;
using HRMS_Backend.Model.GameScheduling;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Backend.Services.GameScheduling
{
    public class EmployeeCycleStatsService : IEmployeeCycleStatsService
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        private readonly IGameCycleService _gameCycleService;
        public EmployeeCycleStatsService(MyDbContext context, IMapper mapper , IGameCycleService gameCycleService)
        {
            _context = context;
            _mapper = mapper;
            _gameCycleService = gameCycleService;
        }
        public async Task<EmployeeCycleStats> createEmployeeCycleStats(EmployeeCycleStatsCreateUpdateDto dto)
        {
            var employeeCycleStats = new EmployeeCycleStats
            {
                GameCycleId = dto.GameCycleId,
                GamePlayed = dto.GamePlayed
            };
            _context.EmployeeCycleStats.Add(employeeCycleStats);
            await _context.SaveChangesAsync();
            return employeeCycleStats;
        }

  
        public async Task<EmployeeCycleStatsDisplayDto> GetUserCycleStatsAsync(int userId, int cycleId)
        {
            var cyclestatus = await _context.EmployeeCycleStats
                .FirstOrDefaultAsync(es => es.UserId == userId && es.GameCycleId == cycleId);
            return _mapper.Map<EmployeeCycleStatsDisplayDto>(cyclestatus);
        }

        public async Task<Boolean> IncrementCompletedPlayCountAsync(List<int> userIds, int CycleId)
        {
            var stats = await _context.EmployeeCycleStats.Where(es => es.GameCycleId == CycleId && userIds.Contains(es.UserId)).ToListAsync();

            foreach (var stat in stats)
            {
                stat.GamePlayed++;
            }
            await _context.SaveChangesAsync();
            return true;
        }



    }
}
