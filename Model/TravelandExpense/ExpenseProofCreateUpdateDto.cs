using HRMS_Backend.Entities.TravelandExpense;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Backend.Model.TravelandExpense
{
    public class ExpenseProofCreateUpdateDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int TravelExpenseId { get; set; }
        public string ProofDocumentUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
