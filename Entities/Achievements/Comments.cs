using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Backend.Entities.Achievements
{
    public class Comments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public Posts Posts { get; set; }
        public int PostsId { get; set; }
        public int AuthorId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int ParentComment {  get; set; }
        public DateTime CreatedAt { get; set; }
        public Boolean IsDeleted { get; set; }


    }
}
