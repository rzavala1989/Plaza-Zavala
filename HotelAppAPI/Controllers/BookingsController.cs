using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelApp.DataAccess.Context;
using HotelAppDataAccess.Models;
using HotelAppDataAccess.Services;
using HotelAppAPI.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HotelAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IEmailService _emailService;
        private readonly ILogger<BookingsController> _logger;
        private readonly HotelContext _context;

        public BookingsController(IBookingService bookingService, IEmailService emailService, ILogger<BookingsController> logger, HotelContext context)
        {
            _bookingService = bookingService;
            _emailService = emailService;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingModel>>> GetBookings()
        {
            _logger.LogInformation("Getting all bookings.");
            var bookings = await _context.Bookings.ToListAsync();
            _logger.LogInformation($"Found {bookings.Count} bookings.");
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingModel>> GetBooking(int id)
        {
            _logger.LogInformation($"Getting booking with ID: {id}");
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                _logger.LogWarning($"Booking with ID: {id} not found.");
                return NotFound();
            }
            return Ok(booking);
        }

        [HttpPost]
        public async Task<ActionResult<BookingModel>> PostBooking(BookingModel booking)
        {
            if (!await _bookingService.IsRoomAvailable(booking.RoomId, booking.StartDate, booking.EndDate))
            {
                return BadRequest("The room is not available for the selected dates or the guest has overlapping bookings.");
            }

            var createdBooking = await _bookingService.CreateBooking(booking);
            if (createdBooking == null)
            {
                return BadRequest("Unable to create booking.");
            }

            _logger.LogInformation($"Creating new booking for guest ID: {booking.GuestId}");

            // Send booking confirmation email
            var guest = await _context.Guests.FindAsync(booking.GuestId);
            if (guest != null)
            {
                _emailService.SendBookingConfirmationEmail(guest.Email, "Booking Confirmation", "Your booking is confirmed.");
            }

            return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, booking);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(int id, BookingModel booking)
        {
            if (id != booking.BookingId)
            {
                _logger.LogWarning($"ID mismatch: {id} does not match booking ID: {booking.BookingId}");
                return BadRequest();
            }
            _logger.LogInformation($"Updating booking with ID: {id}");
            _context.Entry(booking).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            _logger.LogInformation($"Deleting booking with ID: {id}");
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                _logger.LogWarning($"Booking with ID: {id} not found.");
                return NotFound();
            }
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
