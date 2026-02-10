using HRMS_Backend.Data;
using HRMS_Backend.Entities.Games_Scheduling;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Backend.Services.GameScheduling
{
    public class GamesService : IGamesService
    {
        private readonly MyDbContext _context;
        public GamesService(MyDbContext context) {
            _context = context;
        }

        public async Task<IEnumerable<Games>> GetAllGamesAsync()
        {
            return await _context.Games.ToListAsync();
        }
        public async Task<Games> CreateGameAsync(Games newGame)
        {
            _context.Games.Add(newGame);
            await _context.SaveChangesAsync();
            return newGame;
        }
        public async Task<bool> DeleteGameAsync(int id) { 
            var game = await _context.Games.FindAsync(id);
            if(game == null)
            {
                return false;
            }
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
            return true;
        }
        //public async Task<bool> UpdateGameAsync(int id, Games updatedGame)
        //{
        //    if (id != updatedGame.Id)
        //    {
        //        return false;
        //    }
        //    _context.Entry(updatedGame).State = EntityState.Modified;

        //}    
    }
}
