using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Backend.Entities.Achievements
{
    public class PostInteraction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int PostID {  get; set; }
        public int LikeCount {  get; set; }
        //add all types 
        public int CommentCount { get; set; }
        public DateTime LastUpdatedAt { get; set; }

    }
}
