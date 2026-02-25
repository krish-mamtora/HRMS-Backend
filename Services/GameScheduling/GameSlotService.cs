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
        private readonly IGameCycleService _gameCycleService;
        public GameSlotService(MyDbContext context, IMapper mapper, IGameCycleService gameCycleService)
        {
            _context = context;
            _mapper = mapper;
           _gameCycleService = gameCycleService;
        }
        public async Task<int?> GenerateGameSlotAsync(int gamesId, DateOnly gameDate)
        {
          
            int totalSlotAdded = 0;
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
                var cycleId = await _gameCycleService.GetActiveCycleIdAsync(gamesId);
                var gameSlot = new GameSlots
                {
                    GamesId = gamesId,
                    StartTime = slotStartDateTime,
                    Capacity = gameConfig.Capacity,
                    Assigned = 0,
                    AvailableSeats = gameConfig.Capacity,
                    CycleId = cycleId,
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
        public async Task<IEnumerable<GameSlotsDisplayDto>> GetAllGamesSlotAsync()
        {
            var slots = await _context.GameSlots.ToListAsync();
            return _mapper.Map<IEnumerable<GameSlotsDisplayDto>>(slots);
        }
        public async Task<IEnumerable<GameSlotsDisplayDto>> GetGamesSlotForGameAndDateAsync(int id, DateTime dt)
        {
            dt = dt.Date;
            var slots = await _context.GameSlots.Where(gs => gs.GamesId == id && gs.StartTime.Date == dt.Date).ToListAsync();
            return _mapper.Map<IEnumerable<GameSlotsDisplayDto>>(slots);

        }
        //public async Task<GameSlotsDisplayDto> GetSlotByIdAsync(int slotId)
        //{
            
        //}

        //public async Task<int> GetAvailableSeatCountAsync(int slotId)
        //{

        //}

        //public async Task<Boolean> IsBookingOpenAsync(int slotId)
        //{

        //}

        //public async Task<Boolean> IsSlotCompletedAsync(int slotId)
        //{

        //}
        public async Task<Boolean> UpdateSlotStatus(int slotId, bool status)
        {
            try
            {
                var gameSlot = await _context.GameSlots.FirstOrDefaultAsync(s => s.Id == slotId);

                if(gameSlot != null)
                {
                    gameSlot.IsBookingOpen = status;
                    await _context.SaveChangesAsync();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }
        public async Task<Boolean> CompleteSlotAsync(int slotId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var slot = await _context.GameSlots.Include(s => s.Bookings).ThenInclude(s => s.BookingParticipants).FirstOrDefaultAsync(s => s.Id == slotId);

            if (slot == null)
            {
                throw new Exception("Slot not found");
            }
            if (slot.SlotPlayed)
            {
                throw new Exception("Slot already completed");
            }

            slot.SlotPlayed = true;

            var confirmedBookings = slot.Bookings.Where(b => b.Status == "Confirmed").ToList();

            var playedUserIds = confirmedBookings.SelectMany(b => b.BookingParticipants).Select(p => p.EmpId).Distinct().ToList();

            var stats = await _context.EmployeeCycleStats.Where(x => x.GameCycleId == slot.CycleId && playedUserIds.Contains(x.UserId)).ToListAsync();

            foreach (var stat in stats)
            {
                stat.GamePlayed++;
            }

            foreach (var booking in confirmedBookings)
            {
                booking.Status = "Completed";
                booking.SlotPlayed = true;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }

    }
}
