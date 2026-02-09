using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Backend.Entities.TravelandExpense
{
    public class ExpensePolicy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Category { get; set; } = string.Empty;
        [Required(ErrorMessage = "Maximum Amount is required")]
        public int MaxAmout { get; set; }

    }
}
