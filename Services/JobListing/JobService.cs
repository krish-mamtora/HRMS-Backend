using AutoMapper;
using HRMS_Backend.Data;
using HRMS_Backend.Entities;
using HRMS_Backend.Entities.JobListing;
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
            //var Jobs = await _context.Jobs.ToListAsync();
            var JobDtos = _mapper.Map<IEnumerable<JobResponseDto>>(Jobs);
            return JobDtos;
        }

        //public async Task<JobCreateUpdateDto> CreateJobAsync(JobCreateUpdateDto jobCreateUpdateDto)
        //{
        //    var jobEntity = _mapper.Map<HRMS_Backend.Entities.JobListing.Jobs>(jobCreateUpdateDto);
        //    await _context.Jobs.AddAsync(jobEntity);
        //    await _context.SaveChangesAsync();
        //    return _mapper.Map<JobCreateUpdateDto>(jobEntity);
        //}
        public async Task<JobCreateUpdateDto> CreateJobAsync(JobCreateUpdateDto jobCreateUpdateDto)
        {
            if (jobCreateUpdateDto == null)
                throw new ArgumentNullException(nameof(jobCreateUpdateDto));

            var jobEntity = _mapper.Map<HRMS_Backend.Entities.JobListing.Jobs>(jobCreateUpdateDto);

            try
            {
                await _context.Jobs.AddAsync(jobEntity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error saving job to database.", ex);
            }

            return _mapper.Map<JobCreateUpdateDto>(jobEntity);
        }

        public async Task<JobResponseDto?> GetJobByIdAsync(int id)
        {
            var Jobs = await _context.Jobs.FindAsync(id);
            var JobDtos = _mapper.Map<JobResponseDto>(Jobs);
            return JobDtos;
        }
    }
}
