using AutoMapper;
using HRMS_Backend.Data;
using HRMS_Backend.Model;
using HRMS_Backend.Model.JobListing;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Backend.Services.Jobs
{
    public class JobService : IJobsService
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        public JobService(MyDbContext context , IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IEnumerable<JobResponseDto>> GetAllJobsAsync()
        {
            var Jobs = await _context.Jobs.ToListAsync();
            var JobDtos = _mapper.Map<IEnumerable<JobResponseDto>>(Jobs);
            return JobDtos;
        }

        public async Task CreateJobAsync(JobCreateUpdateDto jobCreateUpdateDto)
        {
            //var todo = _mapper.Map<>
            
        }
    }
}
