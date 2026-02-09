using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Backend.Model.TravelandExpense
{
    public class ExpenseCreateUpdateDto
    {
        public int TravelId { get; set; }

        [Required(ErrorMessage = "Employee ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Employee ID must be a valid positive integer.")]
        public int EmplId { get; set; }

        [Required(ErrorMessage = "Expense type is required.")]
        [Range(1, 500000, ErrorMessage = "Expense type must be a valid positive integer.")]
        public int ExpenseType { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Column(TypeName = "decimal")]
        [Range(1, 100000, ErrorMessage = "Amount must be a positive value.")]
        public decimal Amount { get; set; }

        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters.")]
        public string HrRemarks { get; set; }

    }
}
