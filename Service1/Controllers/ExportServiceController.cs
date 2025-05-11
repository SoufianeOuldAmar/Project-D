using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_D.Data;

namespace Project_D.Controllers
{
    [Route("VluchtenService")]
    [ApiController]
    public class ExportServiceController : ControllerBase
    {
        private readonly FlightExportDbContext _context;

        public ExportServiceController(FlightExportDbContext context)
        {
            _context = context;
        }

        [HttpGet("AllVluchtenExports")]
        public IActionResult GetAllFlightIdsWithLinks()
        {
            try
            {
                var baseUrl = $"http://localhost:5041";

                var result = _context.FlightExportInfos
                .OrderBy(f => f.FlightId)  // Orders by FlightId
                .Take(100)
                .Select(f => new
                {
                    flightId = f.FlightId,
                    detailUrl = $"{baseUrl}/VluchtenService/entry?flightId={f.FlightId}"//&uniqueId={f.UniqueId}"
                })
                .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving flight entries: {ex.Message}");
            }
        }

        [HttpGet("entry")]
        public IActionResult GetFlightEntry([FromQuery] int flightId)//, [FromQuery] int uniqueId)
        {
            try
            {
                var flight = _context.FlightExportInfos
                               .FirstOrDefault(f => f.FlightId == flightId); //&& f.UniqueId == uniqueId);

                if (flight == null)
                {
                    return NotFound($"Flight not found with ID {flightId}"); // and unique ID {uniqueId}");
                }

                return Ok(flight);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving flight entry: {ex.Message}");
            }
        }
    }
}