using AutoMapper;
using HRMS_Backend.Data;
using HRMS_Backend.Model.JobListing;

namespace HRMS_Backend.Services.TravelandExpenses
{
    public class EmployeeTravelService
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        public EmployeeTravelService(MyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        //public async Task<IEnumerable<JobResponseDto>> GetAllAssignmedEmployee()
        //{
        //    var Jobs = await _context.Jobs.ToListAsync();
        //    return _mapper.Map<IEnumerable<JobResponseDto>>(Jobs);
        //}
    }
}
