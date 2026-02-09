using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Backend.Entities.TravelandExpense
{
    public class Expenses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TravelId { get; set; }
        public int EmplId { get; set; }
        public int ExpenseType { get; set; }
        public int Amount { get; set; }
        public string Status {  get; set; }
        public string HrRemarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ApprovedBy { get; set; }
       
    }
}
