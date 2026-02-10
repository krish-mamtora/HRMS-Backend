using HRMS_Backend.Entities.Achievements;
using HRMS_Backend.Entities.JobListing;
using HRMS_Backend.Entities.TravelandExpense;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Backend.Entities
{
    public class User
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiry { get; set; } = DateTime.UtcNow.AddDays(2);
        public ICollection<Jobs> Jobs { get; set; } = new List<Jobs>();
        public ICollection<Posts> Posts { get; set; }
        //public ICollection<Referrals> Referrals { get; set; }
        //public ICollection<ShareEmail> ShareEmail {  get; set; }
        public ICollection<Notification> Notification { get; set; }

        public ICollection<TravelPlan> TravelPlan { get; set; }

    }
}

