using HRMS_Backend.Model.TravelandExpense;

namespace HRMS_Backend.Services.TravelandExpenses
{
    public interface IEmployeeTravelService
    {
        Task<IEnumerable<TravelAssignmentDisplayDto>> GetAllAssignedPlansForEmpIdAsync(int id);

        Task CreateBulkUploadTravelPlanAsync(BulkTravelAssignmentDto dto);

        Task<IEnumerable<TravelAssignmentDisplayDto>> GetAllAssignDetailsAsync();

        Task<TravelAssignmentDisplayDto> GetAssignedTravelPlanByIdAsync(int id);

        Task<List<int>> GetAllEmployeesAssignedToPlanAsync(int id);
    }
}