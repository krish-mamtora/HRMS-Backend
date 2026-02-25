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
            var cyclestatus = await _context.EmployeeCycleStats.FindAsync(cycleId);
            return _mapper.Map<EmployeeCycleStatsDisplayDto>(cyclestatus);
        }
    }
}
