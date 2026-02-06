using System.ComponentModel.DataAnnotations;

namespace HRMS_Backend.Entities.Games_Scheduling
{
    public class GameSlots
    {
        [Key]
        public int Id { get; set; }
        public Games Games { get; set; }
        public int GamesId { get; set; }
        public DateTime StartTime {  get; set; }
        public DateTime EndTime { get; set; }
        public Boolean IsAvailable { get; set; }

    }
}
