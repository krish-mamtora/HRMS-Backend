namespace HRMS_Backend.Model.GameScheduling
{
    public class GameSlotsDisplayDto
    {
        public int Id { get; set; }
        public int GamesId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Boolean IsAvailable { get; set; }

    }
}
