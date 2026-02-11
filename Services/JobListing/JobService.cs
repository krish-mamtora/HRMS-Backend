using AutoMapper;
using HRMS_Backend.Data;
using HRMS_Backend.Entities;
using HRMS_Backend.Entities.JobListing;
using HRMS_Backend.Model;
using HRMS_Backend.Model.JobListing;
using HRMS_Backend.Model.TravelandExpense;

//using HRMS_Backend.Services.Jobs;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Backend.Services.JobListing
{
    public class JobService : IJobService
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
            return _mapper.Map<IEnumerable<JobResponseDto>>(Jobs);
        }

        public async Task<IEnumerable<TravelResponseDto>> GetAllPlansAsync()
        {
            var plans = await _context.TravelPlan.ToListAsync();
            return _mapper.Map<IEnumerable<TravelResponseDto>>(plans);
        }
        public async Task<Jobs> CreateJobAsync(JobCreateUpdateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.ManagedBy == null)
                throw new ArgumentException("ManagedBy is required", nameof(dto.ManagedBy));


            var job = new Jobs
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status,
                ExpYearsReq = dto.ExpYearsReq,
                Role = dto.Role,
                JdUrl = dto.JdUrl,
                TotalPositions = dto.TotalPositions,
                ContactMail = dto.ContactMail,
                ManagedBy = dto.ManagedBy,
            };
            _context.Jobs.AddAsync(job);
            await _context.SaveChangesAsync();
            return job;
        }
        public async Task<JobResponseDto?> GetJobByIdAsync(int id)
        {
            var Jobs = await _context.Jobs.FindAsync(id);
            var JobDtos = _mapper.Map<JobResponseDto>(Jobs);
            return JobDtos;
        }
    }
}
