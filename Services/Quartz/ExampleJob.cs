using HRMS_Backend.Entities.Games_Scheduling;
using HRMS_Backend.Services.Achievements;
using HRMS_Backend.Services.GameScheduling;
using Quartz;
using System.ComponentModel.DataAnnotations;

namespace HRMS_Backend.Services.Quartz
{
    public class ExampleJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;

        public ExampleJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //var gameSlotService = scope.ServiceProvider.GetRequiredService<IGameSlotService>();
                //var gamesService = scope.ServiceProvider.GetRequiredService<IGamesService>();
                var postService = scope.ServiceProvider.GetRequiredService<IPostsService>();

                await postService.GenerateSystemPosts();
                await postService.GenerateAnniversaryPosts();

                //var games = await gamesService.GetAllGamesAsync();
                //foreach (var game in games)
                //{
                //    var added = await gameSlotService.GenerateGameSlotAsync(game.Id, DateOnly.FromDateTime(DateTime.Now));
                //    Console.WriteLine($"[Quartz Job] {DateTime.Now}: Added slot: {added}");
                //}
            }
        }
    }
}
