using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelApp.DataAccess.Context;
using HotelAppDataAccess.Models;
using HotelAppLibrary;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HotelAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuestsController : ControllerBase
    {
        private readonly HotelContext _context;
        private readonly ILogger<GuestsController> _logger;

        public GuestsController(HotelContext context, ILogger<GuestsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GuestModel>>> GetGuests()
        {
            _logger.LogInformation("Getting all guests.");
            var guests = await _context.Guests.ToListAsync();
            _logger.LogInformation($"Found {guests.Count} guests.");
            return Ok(guests);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GuestModel>> GetGuest(int id)
        {
            _logger.LogInformation($"Getting guest with ID: {id}");
            var guest = await _context.Guests.FindAsync(id);
            if (guest == null)
            {
                _logger.LogWarning($"Guest with ID: {id} not found.");
                return NotFound();
            }
            return Ok(guest);
        }

        [HttpGet("byEmail/{email}")]
        public async Task<ActionResult<GuestModel>> GetGuestByEmail(string email)
        {
            _logger.LogInformation($"Getting guest with email: {email}");
            var guest = await _context.Guests.FirstOrDefaultAsync(g => g.Email == email);
            if (guest == null)
            {
                _logger.LogWarning($"Guest with email: {email} not found.");
                return NotFound();
            }
            return Ok(guest);
        }

        [HttpGet("search/{query}")]
        public async Task<ActionResult<IEnumerable<GuestModel>>> SearchGuests(string query)
        {
            _logger.LogInformation($"Searching guests with query: {query}");
            var guests = await _context.Guests
                .Where(g => g.FirstName.Contains(query) || g.LastName.Contains(query) || g.Email.Contains(query))
                .ToListAsync();
            _logger.LogInformation($"Found {guests.Count} guests matching query: {query}");
            return Ok(guests);
        }

        [HttpGet("paginated")]
        public async Task<ActionResult<IEnumerable<GuestModel>>> GetPaginatedGuests(int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Getting guests for page number: {pageNumber}, page size: {pageSize}");
            var guests = await _context.Guests
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            _logger.LogInformation($"Found {guests.Count} guests for page number: {pageNumber}");
            return Ok(guests);
        }

        [HttpPost]
        public async Task<ActionResult<GuestModel>> PostGuest(GuestModel guest)
        {
            _logger.LogInformation($"Creating new guest: {guest.FirstName} {guest.LastName}");
            _context.Guests.Add(guest);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetGuest), new { id = guest.GuestModelId }, guest);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutGuest(int id, GuestModel guest)
        {
            if (id != guest.GuestModelId)
            {
                _logger.LogWarning($"ID mismatch: {id} does not match guest ID: {guest.GuestModelId}");
                return BadRequest();
            }
            _logger.LogInformation($"Updating guest with ID: {id}");
            _context.Entry(guest).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGuest(int id)
        {
            _logger.LogInformation($"Deleting guest with ID: {id}");
            var guest = await _context.Guests.FindAsync(id);
            if (guest == null)
            {
                _logger.LogWarning($"Guest with ID: {id} not found.");
                return NotFound();
            }
            _context.Guests.Remove(guest);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
