namespace HRMS_Backend.Model.JobListing
{
    public class JobResponseDto
    {
        public  int Id {  get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description {  get; set; } = string.Empty ;
        public string Status = string.Empty;
        int ExpYearsReq { get; set; }
        public int? ManagedBy { get; set; }
        public string Role = string.Empty;
        int TotalPositions { get; set; }
        public string JdUrl = string.Empty;
        public string ContactMail = string.Empty;
        public string? ManagerName = string.Empty;

    }


}
