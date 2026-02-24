using AutoMapper;
using HRMS_Backend.Data;
using HRMS_Backend.Entities.Games_Scheduling;
using HRMS_Backend.Model.GameScheduling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Backend.Services.GameScheduling
{
    public class GameSlotService : IGameSlotService
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        public GameSlotService(MyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<int?> GenerateGameSlotAsync(int gamesId, DateOnly gameDate)
        {
            int totalSlotAdded = 0;
            //var gameConfig = await _context.GameConfiguration.FindAsync(gamesId);
            var gameConfig = await _context.GameConfiguration.FirstOrDefaultAsync(gc => gc.GamesId == gamesId);
            Console.Write(gameConfig);
            if (gameConfig == null)
            {
                return null;
            }

            var slotDuration = gameConfig.SlotDuration;  // minute ma 
            var startTime = gameConfig.StartTime;       
            var endTime = gameConfig.OverTime;         
            
            var totalMinutes = (endTime- startTime).TotalMinutes;

            if (totalMinutes <= 0)
            {
                return null;
            }
            var currentStart = startTime;
            while (currentStart.AddMinutes(slotDuration)<=endTime)
            {
                var slotStartDateTime = gameDate.ToDateTime(currentStart);
                var slotEndDateTime = slotStartDateTime.AddMinutes(slotDuration);

                var gameSlot = new GameSlots
                {
                    GamesId = gamesId,
                    StartTime = slotStartDateTime,
                    Capacity = gameConfig.Capacity,
                    Assigned = 0,
                    AvailableSeats = gameConfig.Capacity,
                    EndTime = slotEndDateTime,
                    IsBookingOpen = true
                };

                try
                {
                    await _context.GameSlots.AddAsync(gameSlot);
                    await _context.SaveChangesAsync();
                    totalSlotAdded++;
                }
                catch (DbUpdateException ex)
                {
                    var sqlException = ex.GetBaseException() as SqlException;

                    if (sqlException != null && (sqlException.Number == 2601 || sqlException.Number == 2627))
                    {
                     
                        _context.Entry(gameSlot).State = EntityState.Detached;

                    }
                    else
                    {
                        throw;
                    }
                }


                currentStart = currentStart.AddMinutes(slotDuration);
            }

            return totalSlotAdded;
        }

    }
}
