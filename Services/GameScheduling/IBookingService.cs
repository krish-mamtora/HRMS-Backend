using HRMS_Backend.Model.GameScheduling;

namespace HRMS_Backend.Services.GameScheduling
{
    public interface IBookingService
    {
        public Task<BookingResultDto> RequestBookingAsync(BookingRequestCreateDto dto);
        public Task<BookingResultDto> ManageRequestBookingAsync(int slotId, List<int> userIds, int bookedBy);
        public Task<BookingsDisplayDto> getBookingById(int id);

    }
}
