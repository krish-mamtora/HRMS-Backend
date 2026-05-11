using HRMS_Backend.Entities.TravelandExpense;
using HRMS_Backend.Model.TravelandExpense;

namespace HRMS_Backend.Services.TravelandExpenses
{
    public interface IExpenseProofService
    {
        Task<ExpenseProof>CreateExpenseProofAsync(ExpenseProofCreateUpdateDto dto);
        Task<ExpenseProofDisplayDto>GetExpenseProofByIdAsync(int id);
        Task<IEnumerable<ExpenseProofDisplayDto>>GetExpenseProofByExpenseIdAsync(int id);
    }
}