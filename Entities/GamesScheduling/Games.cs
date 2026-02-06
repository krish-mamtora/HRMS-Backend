using System.ComponentModel.DataAnnotations;

namespace HRMS_Backend.Entities.Games_Scheduling
{
    public class Games
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public Boolean IsAvailable { get; set; }
        public ICollection<GameConfiguration> GameConfigurations { get; set; }
        public ICollection<GameSlots> GameSlots { get; set; }
    }
}
