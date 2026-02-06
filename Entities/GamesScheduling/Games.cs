using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Backend.Entities.Games_Scheduling
{
    public class Games
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Game ID is required")]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        [Required(ErrorMessage = "Game ID is required")]
        public string Location { get; set; } = string.Empty;
        [Required]
        public Boolean IsAvailable { get; set; }
        public ICollection<GameConfiguration> GameConfigurations { get; set; }
        public ICollection<GameSlots> GameSlots { get; set; }
    }
}
