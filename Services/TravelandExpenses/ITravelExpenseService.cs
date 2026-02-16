using HRMS_Backend.Entities.TravelandExpense;
using HRMS_Backend.Model.TravelandExpense;

namespace HRMS_Backend.Services.TravelandExpenses
{
    public interface ITravelExpenseService
    {
        Task<TravelExpense> CreateTravelExpenseAsync(ExpenseCreateUpdateDto dto);
        Task<IEnumerable<ExpenseDisplayDto>> GetAllExpenseAsync();
        Task<ExpenseDisplayDto?> GetExpenseByIdAsync(int id);
        Task<IEnumerable<ExpenseDisplayDto>> GetExpenseByTravelAssignmentId(int id);        //Task<bool> UpdatePlanById(int id, ExpenseCreateUpdateDto dto);
                                                                                           //Task<bool> DeletePlanById(int id);
        Task<bool> UpdatePlanExpenseById(int id, ExpenseCreateUpdateDto dto);
    }
}
