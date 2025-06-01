using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_D.Data;
using Microsoft.EntityFrameworkCore;


namespace FlightExportsService.Controllers.V1
{
    [Route("api/v1/flight-exports")]
    [ApiController]
    public class FlightExportsController : ControllerBase
    {
        private readonly FlightExportDbContext _context;

        public FlightExportsController(FlightExportDbContext context)
        {
            _context = context;
        }

        [HttpGet("All-Flights")]
        public async Task<IActionResult> GetAllFlightIdsWithLinks()
        {
            try
            {
                var flights = await _context.FlightExportInfos
                    .OrderBy(f => f.FlightId)
                    .Select(f => new
                    {
                        f.FlightId,
                        detailUrl = $"http://localhost:5041/api/v1/flight-exports/entry?flightId={f.FlightId}"
                    })
                    .ToListAsync();

                return Ok(flights);
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

        [HttpGet("by-airline")]
        public async Task<IActionResult> GetByAirline([FromQuery] string airline)
        {
            if (string.IsNullOrWhiteSpace(airline)) return BadRequest(new { message = "Airline is required." });

            var flights = await _context.FlightExportInfos
                .Where(f => f.AirlineFullname != null && f.AirlineFullname.ToLower() == airline.Trim().ToLower())
                .OrderBy(f => f.FlightId)
                .Take(100)
                .Select(f => new
                {
                    f.FlightId,
                    detailUrl = $"http://localhost:5041/api/v1/flight-exports/entry?flightId={f.FlightId}"
                }).ToListAsync();

            if (!flights.Any()) return NotFound(new { message = "No flights found for airline." });

            return Ok(flights);
        }

        [HttpGet("by-airport")]
        public async Task<IActionResult> GetByAirport([FromQuery] string airportCode)
        {
            if (string.IsNullOrWhiteSpace(airportCode)) return BadRequest(new { message = "Airport code is required." });

            var flights = await _context.FlightExportInfos
                .Where(f => f.Airport != null && f.Airport.ToUpper() == airportCode.Trim().ToUpper())
                .OrderBy(f => f.FlightId)
                .Take(100)
                .Select(f => new
                {
                    f.FlightId,
                    detailUrl = $"http://localhost:5041/api/v1/flight-exports/entry?flightId={f.FlightId}"
                }).ToListAsync();

            if (!flights.Any()) return NotFound(new { message = "No flights found for airport." });

            return Ok(flights);
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