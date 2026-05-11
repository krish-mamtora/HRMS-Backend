using AutoMapper;
using HRMS_Backend.Common.Exceptions;
using HRMS_Backend.Data;
using HRMS_Backend.Entities.TravelandExpense;
using HRMS_Backend.Model.TravelandExpense;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Backend.Services.TravelandExpenses
{
    public class EmployeeTravelService : IEmployeeTravelService
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeTravelService> _logger;

        public EmployeeTravelService(
            MyDbContext context,
            IMapper mapper,
            ILogger<EmployeeTravelService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TravelAssignmentDisplayDto>>
            GetAllAssignDetailsAsync()
        {
            var assignments = await _context.TravelAssignment
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<
                IEnumerable<TravelAssignmentDisplayDto>>(
                assignments);
        }

        public async Task<TravelAssignmentDisplayDto>
            GetAssignedTravelPlanByIdAsync(int id)
        {
            var assignment = await _context.TravelAssignment
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (assignment is null)
            {
                throw new NotFoundException(
                    "Assigned travel plan not found");
            }

            return _mapper.Map<TravelAssignmentDisplayDto>(
                assignment);
        }

        public async Task<IEnumerable<TravelAssignmentDisplayDto>>
            GetAllAssignedPlansForEmpIdAsync(int id)
        {
            var assignments = await _context.TravelAssignment
                .AsNoTracking()
                .Where(x => x.EmpId == id)
                .ToListAsync();

            return _mapper.Map<
                IEnumerable<TravelAssignmentDisplayDto>>(
                assignments);
        }

        public async Task CreateBulkUploadTravelPlanAsync(
            BulkTravelAssignmentDto dto)
        {
            var existingEmpIds = await _context.TravelAssignment
                .Where(x =>
                    x.PId == dto.PId &&
                    dto.EmpId.Contains(x.EmpId))
                .Select(x => x.EmpId)
                .ToListAsync();

            var newEmpIds = dto.EmpId
                .Except(existingEmpIds)
                .ToList();

            if (!newEmpIds.Any())
            {
                throw new BadRequestException(
                    "Employees are already assigned to this plan");
            }

            var assignments = newEmpIds
                .Select(empId => new TravelAssignment
                {
                    EmpId = empId,
                    PId = dto.PId,
                    Status = dto.Status,
                    CreatedAt = DateTime.UtcNow
                })
                .ToList();

            await _context.TravelAssignment
                .AddRangeAsync(assignments);

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Travel plan assigned successfully");
        }

        public async Task<List<int>>
            GetAllEmployeesAssignedToPlanAsync(int id)
        {
            var employeeIds = await _context.TravelAssignment
                .AsNoTracking()
                .Where(x => x.PId == id)
                .Select(x => x.EmpId)
                .ToListAsync();

            return employeeIds;
        }
    }
}