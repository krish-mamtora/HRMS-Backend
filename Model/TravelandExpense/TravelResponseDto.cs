namespace HRMS_Backend.Model.TravelandExpense
{
    public class TravelResponseDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Destination { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}
