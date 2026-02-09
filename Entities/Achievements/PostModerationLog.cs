using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Backend.Entities.Achievements
{
    public class PostModerationLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Type { get; set; }//0 for post n 1 for comment
        public string Action { get; set; }
        public string Reason { get; set; }
        public int ModeratedBy { get; set; }

        public DateTime ModifiedAt { get; set; }

    }
}
