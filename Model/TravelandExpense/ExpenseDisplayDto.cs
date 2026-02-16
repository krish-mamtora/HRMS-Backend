using HRMS_Backend.Entities.TravelandExpense;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Backend.Model.TravelandExpense
{
    public class ExpenseDisplayDto
    {

        public int Id { get; set; }
        public int TravelAssignId { get; set; }
        public int? ExpenseType { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = "pending";
        public string? HrRemarks { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? ApprovedBy { get; set; }

    }
}
