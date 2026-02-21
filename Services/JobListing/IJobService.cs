using HRMS_Backend.Entities;
using HRMS_Backend.Entities.JobListing;
using HRMS_Backend.Model;
using HRMS_Backend.Model.JobListing;

namespace HRMS_Backend.Services.JobListing
{
    public interface IJobService
    {
        Task<IEnumerable<JobResponseDto>> GetAllJobsAsync();
        Task<JobResponseDto?> GetJobByIdAsync(int id);
        Task<Jobs> CreateJobAsync(JobCreateUpdateDto jobDto);
        //Task<JobCreateUpdateDto> UpdateJobAsync(int id, JobCreateUpdateDto jobDto);
        //Task<bool> DeleteJobAsync(int id);
    }
}
