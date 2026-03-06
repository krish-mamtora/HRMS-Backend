using HRMS_Backend.Model.JobListing;

namespace HRMS_Backend.Services.JobListing
{
    public interface IShareEmailService
    {
        public Task<List<ShareMailResponseDto>> getJobShareByUserId(int id);
    }
}
