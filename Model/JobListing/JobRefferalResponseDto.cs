using System.ComponentModel.DataAnnotations;

namespace HRMS_Backend.Model.JobListing
{
    public class JobRefferalResponseDto
    {
        public int JobId { get; set; }

        [StringLength(20)]
        [Required(ErrorMessage = "Refferal Name is required")]
        public string ReffName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string ReffMail { get; set; } = string.Empty;

        [StringLength(100)]
        [Required(ErrorMessage = "Resume is required")]
        public string ReffResumeUrl { get; set; } = string.Empty;
        public int EmpId { get; set; }

        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
