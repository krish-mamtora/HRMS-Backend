using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Backend.Entities.Achievements
{
    public class PostImages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageId { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Image path/URL cannot exceed 255 characters.")]
        public string ImagePath { get; set; } = null!;
        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public Posts Post { get; set; } = null!;
    }
}
