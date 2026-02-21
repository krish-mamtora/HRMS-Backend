using HRMS_Backend.Entities.Games_Scheduling;

namespace HRMS_Backend.Services.GameScheduling
{
    public interface IGamesService
    {
        Task<IEnumerable<Games>> GetAllGamesAsync();
        //Task<Games?> GetGameByIdAsync(int id);
        Task<Games> CreateGameAsync(Games newGame);
        //Task<bool> UpdateGameAsync(int id, Games updatedGame);
        //Task<bool> DeleteGameAsync(int id);
    }
}
