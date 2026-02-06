using HRMS_Backend.Entities.Games_Scheduling;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Backend.Entities.Achievements
{
    public class Tags
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TagName { get; set; }  = string.Empty;

        public ICollection<Posts> Posts { get; set; }
    }
}
