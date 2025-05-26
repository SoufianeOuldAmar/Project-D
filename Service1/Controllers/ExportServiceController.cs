using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_D.Data;
using Microsoft.EntityFrameworkCore;


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

        [HttpGet("flight-statistics")]
        public async Task<IActionResult> GetFlightStatistics(
        [FromQuery] string startDatetime = null,
        [FromQuery] string endDatetime = null)
        {
            // Parse datetime parameters if provided
            DateTime? parsedStart = null;
            DateTime? parsedEnd = null;

            if (!string.IsNullOrWhiteSpace(startDatetime))
            {
                if (!DateTime.TryParse(startDatetime, out var tempStart))
                    return BadRequest(new { message = "Invalid startDatetime format." });
                parsedStart = tempStart;
            }

            if (!string.IsNullOrWhiteSpace(endDatetime))
            {
                if (!DateTime.TryParse(endDatetime, out var tempEnd))
                    return BadRequest(new { message = "Invalid endDatetime format." });
                parsedEnd = tempEnd;
            }

            if (parsedStart != null && parsedEnd != null && parsedStart > parsedEnd)
                return BadRequest(new { message = "startDatetime must be before endDatetime." });

            // Base query with optional date filtering
            var baseQuery = _context.FlightExportInfos.AsQueryable();

            if (parsedStart != null)
                baseQuery = baseQuery.Where(f => f.ScheduledLocal >= parsedStart);

            if (parsedEnd != null)
                baseQuery = baseQuery.Where(f => f.ScheduledLocal <= parsedEnd);

            var statistics = new
            {
                // Date range used for the statistics
                StartDate = parsedStart,
                EndDate = parsedEnd,

                // Delayed flights (where ActualLocal > ScheduledLocal)
                DelayedFlights = await baseQuery
                    .CountAsync(f => f.ScheduledLocal != null &&
                                f.ActualLocal != null &&
                                f.ActualLocal > f.ScheduledLocal),

                // Arriving flights (assuming TrafficType "A" is arrivals)
                ArrivingFlights = await baseQuery
                    .CountAsync(f => f.TrafficType != null &&
                                f.TrafficType.ToUpper() == "A"),

                // Diverted flights
                DivertedFlights = await baseQuery
                    .CountAsync(f => f.Diverted != null &&
                                f.Diverted),

                // Nachtvlucht flights
                NachtvluchtFlights = await baseQuery
                    .CountAsync(f => f.Nachtvlucht != null &&
                                f.Nachtvlucht),

                // Most popular airport (top 5)
                MostPopularAirports = await baseQuery
                    .Where(f => f.AirportIata != null)
                    .GroupBy(f => f.AirportIata)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => new
                    {
                        Airport = g.Key,
                        FlightCount = g.Count()
                    })
                    .ToListAsync(),

                // Total seats
                TotalSeats = await baseQuery
                    .SumAsync(f => f.Seats ?? 0),

                // EU vs non-EU flights
                EuFlights = await baseQuery
                    .CountAsync(f => f.Eu != null &&
                                f.Eu),
                NonEuFlights = await baseQuery
                    .CountAsync(f => f.Eu != null &&
                                f.Eu == false),

                // Passenger statistics
                PaxStatistics = new
                {
                    Male = await baseQuery.SumAsync(f => f.PaxMale ?? 0),
                    Female = await baseQuery.SumAsync(f => f.PaxFemale ?? 0),
                    Infant = await baseQuery.SumAsync(f => f.PaxInfant ?? 0),
                    Child = await baseQuery.SumAsync(f => f.PaxChild ?? 0),
                    Total = await baseQuery.SumAsync(f => f.TotaalPax ?? 0),
                    Terminal = await baseQuery.SumAsync(f => f.TerminalPax ?? 0),
                    Transit = await baseQuery.SumAsync(f => f.TransitPax ?? 0)
                },

                // Baggage statistics
                BaggageStatistics = new
                {
                    TotalWeight = await baseQuery.SumAsync(f => f.TotaalBagsWeight ?? 0),
                    TerminalWeight = await baseQuery.SumAsync(f => f.TerminalBagsWeight ?? 0),
                    TransitWeight = await baseQuery.SumAsync(f => f.TransitBagsWeight ?? 0),
                    TotalBags = await baseQuery.SumAsync(f => f.TotaalBags ?? 0),
                    TerminalBags = await baseQuery.SumAsync(f => f.TerminalBags ?? 0),
                    TransitBags = await baseQuery.SumAsync(f => f.TransitBags ?? 0)
                },

                // Flight count
                TotalFlights = await baseQuery.CountAsync()
            };

            return Ok(statistics);
        }
    }
}