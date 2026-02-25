using HRMS_Backend.Entities.GamesScheduling;
using HRMS_Backend.Model.GameScheduling;

namespace HRMS_Backend.Services.GameScheduling
{
    public interface IGameCycleService
    {
        Task<IEnumerable<GameCycle>> GetAllGameCyclesAsync();
        Task<GameCycle> GetGameCycleByIdAsync(int id);
        Task<GameCycle> CreateGameCycleAsync(GameConfigCreateUpdateDto dto);

        Task<int> GetActiveCycleIdAsync(int id);
    }
}
