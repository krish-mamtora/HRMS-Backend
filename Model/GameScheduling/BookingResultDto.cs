namespace HRMS_Backend.Model.GameScheduling
{
    public class BookingResultDto
    {
        public List<int> BookedUsers { get; set; } = new List<int>();
        public List<int> WaitingUsers { get; set; } = new List<int>();
    }
}
