using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Backend.Entities.JobListing
{
    public class Referals
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int JobId { get; set; }
        //[ForeignKey("JobId")]
        public Jobs Job { get; set; }


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
        //[ForeignKey("EmpId")]
        public User Employee { get; set; }

        [StringLength(20)]
        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = "refered";

        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}