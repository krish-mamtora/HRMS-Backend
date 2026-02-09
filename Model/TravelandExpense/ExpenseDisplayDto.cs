namespace HRMS_Backend.Model.TravelandExpense
{
    public class ExpenseDisplayDto
    {
        public int Id { get; set; }
        public int TravelId { get; set; }
        public int EmplId { get; set; }
        public int ExpenseType { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string HrRemarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ApprovedBy { get; set; }

    }
}
