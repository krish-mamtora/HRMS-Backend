using HRMS_Backend.Model.GameScheduling;

namespace HRMS_Backend.Services.GameScheduling
{
    public interface IBookingService
    {
        public Task<BookingResultDto> RequestBookingAsync(BookingRequestCreateDto dto);
        public Task<BookingsDisplayDto> getBookingById(int id);
        public Task<IEnumerable<BookingsDisplayDto>> getBookingsByUserId(int id);

        public Task<Boolean> CancelBooking(int bookingId);

    }
}
