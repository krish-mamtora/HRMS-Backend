using HRMS_Backend.Entities;
using HRMS_Backend.Model;
using HRMS_Backend.Model.JobListing;

namespace HRMS_Backend.Services.Jobs
{
    public interface IJobsService
    {
        Task<IEnumerable<JobResponseDto>> GetAllJobsAsync();
        Task<JobResponseDto> GetJobByIdAsync(int id);
        Task<JobCreateUpdateDto> CreateJobAsync(JobCreateUpdateDto jobDto);
        Task<JobCreateUpdateDto> UpdateJobAsync(int id, JobCreateUpdateDto jobDto);
        Task<bool> DeleteJobAsync(int id);
    }
}
