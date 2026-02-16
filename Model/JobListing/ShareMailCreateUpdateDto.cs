using HRMS_Backend.Entities;
using HRMS_Backend.Entities.JobListing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Backend.Model.JobListing
{
    public class ShareMailCreateUpdateDto
    {
        public int Id { get; set; }
        public int JobId { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string ReceiverMail { get; set; } = string.Empty;
        public int EmpId { get; set; }

        [StringLength(100)]
        public string Subject { get; set; } = string.Empty;

        [StringLength(200)]
        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
