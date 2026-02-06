using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HRMS_Backend.Entities.Games_Scheduling
{
    public class GameConfiguration
    {
        [Key]
        public int Id { get; set; }
        public Games Games { get; set; }
        public int GamesId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly OverTime { get; set; }
        public int SlotDuration { get; set; }
        public int Capacity { get; set; }


    }
}
