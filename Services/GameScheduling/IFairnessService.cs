namespace HRMS_Backend.Services.GameScheduling
{
    public interface IFairnessService
    {
        public Task<Boolean> IsUsersEligibleAsync(int slotId, List<int> userIds);
    }
}
