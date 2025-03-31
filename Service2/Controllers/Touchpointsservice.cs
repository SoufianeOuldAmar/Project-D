using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service2.Data;

namespace Service2.Controllers
{
    [Route("TouchpointsService")]
    [ApiController]
    public class Touchpointsservice : ControllerBase
    {
        private readonly Vlucht2024TouchpointDbContext _context;

        public Touchpointsservice(Vlucht2024TouchpointDbContext context)
        {
            _context = context;
        }

        [HttpGet("AllVluchtenTouchpoints")]
        public IActionResult GetAllFlightTouchpointsLink()
        {
            try
            {

                var baseUrl = $"http://localhost:5153";


                var result = _context.TouchpointInfos
                    .GroupBy(f => f.FlightId)
                    .Select(g => new
                    {
                        flightId = g.Key,
                        detailUrls = g.Select(f => $"{baseUrl}/TouchpointsService/entry?flightId={f.FlightId}&uniqueId={f.UniqueId}")
                                        .ToList(),
                        count = g.Count()
                    })
                    .OrderBy(x => x.flightId)
                    //.Take(100) // Limit to 100 groups
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving flight data: {ex.Message}");
            }
        }

        [HttpGet("entry")]
        public IActionResult GetFlightEntry([FromQuery] int flightId, [FromQuery] int UniqueId)
        {
            try
            {
                var flight = _context.TouchpointInfos
                                .FirstOrDefault(f => f.FlightId == flightId && f.UniqueId == UniqueId);

                if (flight == null)
                {
                    return NotFound($"Flight not found with ID {flightId} and unique ID {UniqueId}");
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
