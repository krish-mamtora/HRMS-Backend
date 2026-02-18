using HRMS_Backend.Model.TravelandExpense;

public interface ITravelExpenseService
{
    Task<TravelExpense> CreateTravelExpenseAsync(ExpenseCreateUpdateDto dto);
    Task<IEnumerable<ExpenseDisplayDto>> GetAllExpenseAsync();
    Task<ExpenseDisplayDto?> GetExpenseByIdAsync(int id);
    Task<ExpenseDisplayDto?> GetExpenseByTravelAssignmentId(int id);
   
}
}