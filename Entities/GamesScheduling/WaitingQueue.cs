using HRMS_Backend.Entities.Games_Scheduling;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Backend.Entities.GamesScheduling
{
    public class WaitingQueue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QueueId { get; set; }

        [Required]
        public int BookingId { get; set; }
        [ForeignKey("BookingId")]
        public Bookings Bookings { get; set; }


        [Required]
        public int SlotId { get; set; }
        [ForeignKey("SlotId")]
        public GameSlots GameSlots { get; set; }


        [Required]
        public int CycleId { get; set; }
        [ForeignKey("CycleId")]
        public GameCycle GameCycle { get; set; }

        public DateTime InsertionTime = DateTime.Now;
        public DateTime UpdationTime = DateTime.Now;

        public string Status { get; set; }

    }
}
