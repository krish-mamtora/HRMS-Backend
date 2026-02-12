using AutoMapper;
using HRMS_Backend.Data;
using HRMS_Backend.Entities.TravelandExpense;
using HRMS_Backend.Model.JobListing;
using HRMS_Backend.Model.TravelandExpense;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Backend.Services.TravelandExpenses
{
    public class EmployeeTravelService : IEmployeeTravelService
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        public EmployeeTravelService(MyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IEnumerable<TravelAssignmentDisplayDto>> getAllAssignDetails()
        {
            var Jobs = await _context.TravelAssignment.ToListAsync();
            return _mapper.Map<IEnumerable<TravelAssignmentDisplayDto>>(Jobs);
        }
        public async Task<TravelAssignmentDisplayDto?> getAssignedTravelPlayById(int id)
        {
            var assignedPlan = await _context.TravelAssignment.FindAsync(id);
            var assignedPlanDto = _mapper.Map<TravelAssignmentDisplayDto>(assignedPlan);
            return assignedPlanDto;
        }

        public async Task<IEnumerable<TravelAssignmentDisplayDto>> getAllAssignedPlansForEmpId(int id)
        {
            var assignedPlans = await _context.TravelAssignment.Where(ta => ta.EmpId == id).ToListAsync();
            var assignedPlansDto = _mapper.Map<IEnumerable<TravelAssignmentDisplayDto>>(assignedPlans);
            return assignedPlansDto;

        }

        public async Task<bool> createBulkUploadTravelPlan(BulkTravelAssignmentDto dto)
        {
            var assignments = new List<TravelAssignment>();
            var now = DateTime.UtcNow;

            foreach (var empId in dto.EmpId)
            {
                assignments.Add(new TravelAssignment
                {
                    EmpId = empId,
                    PId = dto.PId,
                    Status = dto.Status,
                    CreatedAt = now,
                    LastUpdatedAt = null,
                });
            }
            await _context.TravelAssignment.AddRangeAsync(assignments);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
