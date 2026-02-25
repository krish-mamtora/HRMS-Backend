using HRMS_Backend.Entities.GamesScheduling;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Backend.Entities.Games_Scheduling
{
    [Index(nameof(GamesId), nameof(StartTime), IsUnique = true)]
    public class GameSlots
    {
        //[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public Games Games { get; set; }

        [Required(ErrorMessage = "Game ID is required")]
        public int GamesId { get; set; }

        [Required(ErrorMessage = "Start Time is required")]
        public DateTime StartTime {  get; set; }

        [Required(ErrorMessage = "End Time is required")]
        public DateTime EndTime { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
        public ICollection<Bookings> Bookings { get; set; }

        public ICollection<WaitingQueue> WaitingQueue { get; set; }

    }
}
