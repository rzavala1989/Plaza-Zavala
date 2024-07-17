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
    public class HotelsController : ControllerBase
    {
        private readonly HotelContext _context;
        private readonly ILogger<HotelsController> _logger;

        public HotelsController(HotelContext context, ILogger<HotelsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelModel>>> GetHotels()
        {
            _logger.LogInformation("Getting all hotels.");
            var hotels = await _context.Hotels.ToListAsync();
            _logger.LogInformation($"Found {hotels.Count} hotels.");
            return Ok(hotels);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HotelModel>> GetHotel(int id)
        {
            _logger.LogInformation($"Getting hotel with ID: {id}");
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                _logger.LogWarning($"Hotel with ID: {id} not found.");
                return NotFound();
            }
            return Ok(hotel);
        }

        [HttpGet("byName/{name}")]
        public async Task<ActionResult<IEnumerable<HotelModel>>> GetHotelsByName(string name)
        {
            _logger.LogInformation($"Getting hotels with name: {name}");
            var hotels = await _context.Hotels.Where(h => h.Name.Contains(name)).ToListAsync();
            _logger.LogInformation($"Found {hotels.Count} hotels with name: {name}");
            return Ok(hotels);
        }

        [HttpGet("search/{query}")]
        public async Task<ActionResult<IEnumerable<HotelModel>>> SearchHotels(string query)
        {
            _logger.LogInformation($"Searching hotels with query: {query}");
            var hotels = await _context.Hotels
                .Where(h => h.Name.Contains(query) || h.Address.Contains(query))
                .ToListAsync();
            _logger.LogInformation($"Found {hotels.Count} hotels matching query: {query}");
            return Ok(hotels);
        }

        [HttpGet("paginated")]
        public async Task<ActionResult<IEnumerable<HotelModel>>> GetPaginatedHotels(int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Getting hotels for page number: {pageNumber}, page size: {pageSize}");
            var hotels = await _context.Hotels
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            _logger.LogInformation($"Found {hotels.Count} hotels for page number: {pageNumber}");
            return Ok(hotels);
        }

        [HttpPost]
        public async Task<ActionResult<HotelModel>> PostHotel(HotelModel hotel)
        {
            _logger.LogInformation($"Creating new hotel: {hotel.Name}");
            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetHotel), new { id = hotel.HotelId }, hotel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, HotelModel hotel)
        {
            if (id != hotel.HotelId)
            {
                _logger.LogWarning($"ID mismatch: {id} does not match hotel ID: {hotel.HotelId}");
                return BadRequest();
            }
            _logger.LogInformation($"Updating hotel with ID: {id}");
            _context.Entry(hotel).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            _logger.LogInformation($"Deleting hotel with ID: {id}");
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                _logger.LogWarning($"Hotel with ID: {id} not found.");
                return NotFound();
            }
            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
