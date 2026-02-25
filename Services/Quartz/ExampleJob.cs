using HRMS_Backend.Entities.Games_Scheduling;
using HRMS_Backend.Services.GameScheduling;
using Quartz;
using System.ComponentModel.DataAnnotations;

namespace HRMS_Backend.Services.Quartz
{
    public class ExampleJob : IJob
    {       
        private readonly IGameSlotService _gameSlotService;
        private readonly IGamesService _gamesService;
        public ExampleJob(IGameSlotService gameSlotService ,IGamesService gamesService)
        {
            _gameSlotService = gameSlotService;
            _gamesService = gamesService;
        }
        public async Task  Execute(IJobExecutionContext context)
        {

            var games = await _gamesService.GetAllGamesAsync(); 

            foreach (var game in games)
            {

                var added = await _gameSlotService.GenerateGameSlotAsync(game.Id, DateOnly.FromDateTime(DateTime.Now));
                Console.WriteLine(added);
                Console.WriteLine("ExampleJob is running: " + DateTime.Now + " And :  "+added+" slot added ");
            }

            //return Task.CompletedTask;
        }
    }
}
