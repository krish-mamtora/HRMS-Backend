using System.ComponentModel.DataAnnotations;

namespace HRMS_Backend.Model.JobListing
{
    public class JobCreateUpdateDto
    {
        [StringLength(50)]
        [Required(ErrorMessage = "Title is required")]
        string Title { get; set; } = string.Empty;

         [StringLength(300)]
        [Required(ErrorMessage = "Description is required")]
        string Description {  get; set; } = string.Empty;

        int? ManagedBy { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "Role is required")]
        string Role {  get; set; } = string.Empty;

        [Required]
        int TotalPositions { get; set; }

        [Range(0, 50, ErrorMessage = "Experience Should be in range 0 to 50")]
        int ExpYearsReq { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "Job Description is required")]
        string JdUrl { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        string ContactMail { get; set; }

    }
}
