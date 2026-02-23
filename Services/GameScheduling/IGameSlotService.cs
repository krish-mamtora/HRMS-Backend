using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Services.GameScheduling
{
    public interface IGameSlotService
    {
        public Task<int?> GenerateGameSlotAsync(int GamesId, DateOnly gameDate);
    }

}
