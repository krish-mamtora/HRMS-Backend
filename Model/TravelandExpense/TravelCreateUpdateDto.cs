using System.ComponentModel.DataAnnotations;

namespace HRMS_Backend.Model.TravelandExpense
{
    public class TravelCreateUpdateDto
    {
        [Required(ErrorMessage = "Start date is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        public DateTime EndDate { get; set; }


        [Required(ErrorMessage = "Destination is required.")]
        [StringLength(100, ErrorMessage = "Destination cannot exceed 100 characters.")]
        public string Destination { get; set; } = string.Empty;

        [Required(ErrorMessage = "Purpose is required.")]
        [StringLength(500, ErrorMessage = "Purpose cannot exceed 500 characters.")]
        public string Purpose { get; set; } = string.Empty;

        [Required(ErrorMessage = "user ID is required.")]
      
        [Range(1, int.MaxValue, ErrorMessage = "Invalid user ID.")]
        public int CreatedByUserId { get; set; }
    }
}
