using HRMS_Backend.Entities.Achievements;
using HRMS_Backend.Entities.JobListing;
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
        public string PasswordHash { get; set; } = string.Empty;
        public string Roles { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiry { get; set; } = DateTime.UtcNow.AddDays(2);
        public ICollection<Jobs> Jobs { get; set; }
        public ICollection<Posts> Posts { get; set; }
        public ICollection<Referrals> Referrals { get; set; }
        public ICollection<ShareEmail> ShareEmail {  get; set; }


    }
}

