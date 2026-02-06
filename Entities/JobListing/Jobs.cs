using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Backend.Entities.JobListing
{
    public class Jobs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        [StringLength(100)]
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;
        [StringLength(20)]
        public string Status { get; set; } = "Open";
        [Range(1,50 , ErrorMessage ="Experiance Age Should be less then 50")]
        public int ExpYearsReq { get; set; }
        [Required]

        [ForeignKey("ManagedBy")]
        public User User { get; set; }
        public int? ManagedBy {  get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty;
        [Required]
        public int TotalPositions { get; set; } = 1;
        public string ContactMail { get; set; } = "ab@gmail.com";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
