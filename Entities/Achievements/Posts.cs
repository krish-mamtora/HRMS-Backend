using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Backend.Entities.Achievements
{
    public class Posts
    {
        [Key]
        public int Id {  get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Title is Required")]

        public string Title { get; set; } = string.Empty;
        [StringLength(150)]
        public string Description { get; set; } =  string.Empty ;
        public Tags Tags { get; set; }
        public int TagsId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("Moderator")]
        public int ModeratedByUserId { get; set; }
        public Boolean IsDeleted { get; set; }
       
        public ICollection<Comments> Comments { get; set; }
    }
}
