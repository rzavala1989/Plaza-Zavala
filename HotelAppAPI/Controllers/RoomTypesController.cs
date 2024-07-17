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
    public class RoomTypesController : ControllerBase
    {
        private readonly HotelContext _context;
        private readonly ILogger<RoomTypesController> _logger;

        public RoomTypesController(HotelContext context, ILogger<RoomTypesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomTypeModel>>> GetRoomTypes()
        {
            _logger.LogInformation("Getting all room types.");
            var roomTypes = await _context.RoomTypes.ToListAsync();
            _logger.LogInformation($"Found {roomTypes.Count} room types.");
            return Ok(roomTypes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomTypeModel>> GetRoomType(int id)
        {
            _logger.LogInformation($"Getting room type with ID: {id}");
            var roomType = await _context.RoomTypes.FindAsync(id);
            if (roomType == null)
            {
                _logger.LogWarning($"Room type with ID: {id} not found.");
                return NotFound();
            }
            return Ok(roomType);
        }

        [HttpGet("byTypeName/{typeName}")]
        public async Task<ActionResult<IEnumerable<RoomTypeModel>>> GetRoomTypesByTypeName(string typeName)
        {
            _logger.LogInformation($"Getting room types with name: {typeName}");
            var roomTypes = await _context.RoomTypes.Where(rt => rt.TypeName.Contains(typeName)).ToListAsync();
            _logger.LogInformation($"Found {roomTypes.Count} room types with name: {typeName}");
            return Ok(roomTypes);
        }

        [HttpGet("search/{query}")]
        public async Task<ActionResult<IEnumerable<RoomTypeModel>>> SearchRoomTypes(string query)
        {
            _logger.LogInformation($"Searching room types with query: {query}");
            var roomTypes = await _context.RoomTypes
                .Where(rt => rt.TypeName.Contains(query) || rt.Description.Contains(query))
                .ToListAsync();
            _logger.LogInformation($"Found {roomTypes.Count} room types matching query: {query}");
            return Ok(roomTypes);
        }

        [HttpGet("paginated")]
        public async Task<ActionResult<IEnumerable<RoomTypeModel>>> GetPaginatedRoomTypes(int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Getting room types for page number: {pageNumber}, page size: {pageSize}");
            var roomTypes = await _context.RoomTypes
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            _logger.LogInformation($"Found {roomTypes.Count} room types for page number: {pageNumber}");
            return Ok(roomTypes);
        }

        [HttpPost]
        public async Task<ActionResult<RoomTypeModel>> PostRoomType(RoomTypeModel roomType)
        {
            _logger.LogInformation($"Creating new room type: {roomType.TypeName}");
            _context.RoomTypes.Add(roomType);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRoomType), new { id = roomType.RoomTypeId }, roomType);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoomType(int id, RoomTypeModel roomType)
        {
            if (id != roomType.RoomTypeId)
            {
                _logger.LogWarning($"ID mismatch: {id} does not match room type ID: {roomType.RoomTypeId}");
                return BadRequest();
            }
            _logger.LogInformation($"Updating room type with ID: {id}");
            _context.Entry(roomType).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomType(int id)
        {
            _logger.LogInformation($"Deleting room type with ID: {id}");
            var roomType = await _context.RoomTypes.FindAsync(id);
            if (roomType == null)
            {
                _logger.LogWarning($"Room type with ID: {id} not found.");
                return NotFound();
            }
            _context.RoomTypes.Remove(roomType);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
