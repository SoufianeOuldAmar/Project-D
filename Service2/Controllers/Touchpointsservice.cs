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
        public IActionResult AllFlightTouchpoints()
        {
            try
            {

                var baseUrl = $"http://localhost:5153";
                var otherUrl = $"http://localhost:5041";


                var result = _context.TouchpointInfos
                    .GroupBy(f => f.FlightId)
                    .Select(g => new
                    {
                        flightId = g.Key,
                        flightUrl = $"{otherUrl}/VluchtenService/entry?flightId={g.Key}",
                        touchpoints = g.Select(f => new
                        {
                            idActual = f.IDActual,
                            detailUrl = $"{baseUrl}/TouchpointsService/entry?flightId={f.FlightId}&idActual={f.IDActual}"
                        }).ToList(),
                        count = g.Count()
                    })
                    .OrderBy(x => x.flightId)
                    .Take(50)
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving flight data: {ex.Message}");
            }
        }

        [HttpGet("AlltouchpointsFlights")]
        public IActionResult AllTouchpointsFlights()
        {
            try
            {

                var baseUrl = $"http://localhost:5153";
                var otherUrl = $"http://localhost:5041";


                var result = _context.TouchpointInfos
                    .OrderBy(f => f.IDActual)
                    .Select(f => new
                    {
                        idActual = f.IDActual,
                        touchpointUrl = $"{baseUrl}/TouchpointsService/entry?flightId={f.FlightId}&idActual={f.IDActual}",
                        flightId = f.FlightId,
                        flightUrl = $"{otherUrl}/VluchtenService/entry?flightId={f.FlightId}"
                    })
                    .ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving flight data: {ex.Message}");
            }
        }

        [HttpGet("entry")]
        public IActionResult GetFlightEntry([FromQuery] int flightId, [FromQuery] int IDActual)
        {
            try
            {
                var flight = _context.TouchpointInfos
                                .FirstOrDefault(f => f.FlightId == flightId && f.IDActual == IDActual);

                if (flight == null)
                {
                    return NotFound($"Flight not found with ID {flightId} and unique ID {IDActual}");
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
