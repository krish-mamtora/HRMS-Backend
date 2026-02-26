using HRMS_Backend.Model.GameScheduling;
using HRMS_Backend.Services.GameScheduling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers.GameScheduling
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _service;
        public  BookingController(IBookingService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> RequestBookingAsync([FromBody] BookingRequestCreateDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Request body cannot be null");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var BookingResult = await _service.RequestBookingAsync(dto);
                return Ok(BookingResult);
                //return CreatedAtAction(nameof(getBookingById), new { id = CreateBooking.BId }, CreateBooking);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{id}", Name = "getBookingById")]
        public async Task<IActionResult> getBookingById(int id)
        {
            var booking = await _service.getBookingById(id);
            if (booking == null)
            {
                return  NotFound();
            }
            return Ok(booking);
        }
        [HttpGet("user/{id}" , Name ="getUserBookings")]
        public async Task<IActionResult> getBookingsByUserId(int id)
        {
            var bookings = await _service.getBookingsByUserId(id);
            if(bookings == null)
            {
                return BadRequest(ModelState);
            }
            return Ok(bookings);
        }

       
        [HttpDelete("{bookingId}", Name = "CancelBooking")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            if (bookingId <= 0)
            {
                return BadRequest("Invalid booking ID.");
            }
            var result = await _service.CancelBooking(bookingId);
            if (!result)
            {
                return NotFound(new { Message = $"Booking with ID {bookingId} not found." });
            }
            return Ok(new { Message = $"Booking {bookingId} cancelled" });
        }
    }
}
