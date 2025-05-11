using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service2.Data;

namespace Service2.Controllers
{
    [Route("TouchpointsService")]
    [ApiController]
    public class Touchpointsservice : ControllerBase
    {
        private readonly FlightTouchpointDbContext _context;

        public Touchpointsservice(FlightTouchpointDbContext context)
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


                var result = _context.FlightTouchpointInfos
                    .GroupBy(f => f.FlightId)
                    .Select(g => new
                    {
                        flightId = g.Key,
                        flightUrl = $"{otherUrl}/VluchtenService/entry?flightId={g.Key}",
                        touchpoints = g.Select(f => new
                        {
                            idActual = f.Idactual,
                            detailUrl = $"{baseUrl}/TouchpointsService/entry?flightId={f.FlightId}&idActual={f.Idactual}"
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


                var result = _context.FlightTouchpointInfos
                    .OrderBy(f => f.Idactual)
                    .Select(f => new
                    {
                        idActual = f.Idactual,
                        touchpointUrl = $"{baseUrl}/TouchpointsService/entry?flightId={f.FlightId}&idActual={f.Idactual}",
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
                var flight = _context.FlightTouchpointInfos
                                .FirstOrDefault(f => f.FlightId == flightId && f.Idactual == IDActual);

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
