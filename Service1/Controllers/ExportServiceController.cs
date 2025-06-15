using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Project_D.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace FlightExportsService.Controllers.V1
{
    [Route("api/v1/flight-exports")]
    [ApiController]
    public class FlightExportsController : ControllerBase
    {
        private readonly FlightExportDbContext _context;
        private readonly IMemoryCache _cache;

        public FlightExportsController(FlightExportDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet("all-flights-with-ids")]
        public async Task<IActionResult> GetAllFlightsWithIds()
        {
            try
            {
                string cacheKey = "AllFlightsWithIds";
                if (_cache.TryGetValue(cacheKey, out List<int> cachedIds))
                {
                    var cachedResponse = cachedIds
                        .Select(fid => new
                        {
                            FlightId = fid,
                            DetailUrl = Url.Action(
                                action: nameof(GetByFlightId),
                                controller: "FlightExports",
                                values: new { flightId = fid }
                            )
                        })
                        .ToList();

                    return Ok(cachedResponse);
                }

                var allFlightIds = await _context.FlightExportInfos
                    .AsNoTracking()
                    .OrderBy(f => f.FlightId)
                    .Select(f => f.FlightId)
                    .ToListAsync();

                if (!allFlightIds.Any())
                {
                    var empty = new
                    {
                        Message = "No flights found in the system."
                    };
                    _cache.Set(cacheKey, new List<int>(), TimeSpan.FromMinutes(10));
                    return Ok(empty);
                }

                var response = allFlightIds
                    .Select(fid => new
                    {
                        FlightId = fid,
                        DetailUrl = Url.Action(
                            action: nameof(GetByFlightId),
                            controller: "FlightExports",
                            values: new { flightId = fid }
                        )
                    })
                    .ToList();

                _cache.Set(cacheKey, allFlightIds, TimeSpan.FromMinutes(10));
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving all flights: {ex.Message}");
            }
        }

        [HttpGet("by-flight-id")]
        public async Task<IActionResult> GetByFlightId(int flightId)
        {
            string cacheKey = $"Flight_{flightId}";
            if (_cache.TryGetValue(cacheKey, out FlightExportInfo cachedFlight))
            {
                return Ok(cachedFlight);
            }

            var flight = await _context.FlightExportInfos
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FlightId == flightId);

            if (flight == null)
                return NotFound($"Flight not found with ID {flightId}.");

            _cache.Set(cacheKey, flight, TimeSpan.FromMinutes(10));
            return Ok(flight);
        }

        [HttpGet("by-airline-full-name/{airlineName}")]
        public async Task<IActionResult> GetByAirlineFullName(string airlineName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(airlineName))
                    return BadRequest(new { Message = "Airline full name is required." });

                airlineName = airlineName.Trim().ToLower();
                string cacheKey = $"FlightsByAirline_{airlineName}";

                if (_cache.TryGetValue(cacheKey, out List<int> cachedIds))
                {
                    var cachedResponse = cachedIds
                        .Select(fid => new
                        {
                            FlightId = fid,
                            DetailUrl = Url.Action(
                                action: nameof(GetByFlightId),
                                controller: "FlightExports",
                                values: new { flightId = fid }
                            )
                        })
                        .ToList();

                    return Ok(cachedResponse);
                }

                var matchingIds = await _context.FlightExportInfos
                    .AsNoTracking()
                   .Where(f =>
                        f.AirlineFullname != null &&
                        f.AirlineFullname.ToLower() == airlineName)
                    .Select(f => f.FlightId)
                    .OrderBy(id => id)
                    .Take(100)
                    .ToListAsync();

                if (!matchingIds.Any())
                {
                    var emptyResult = new
                    {
                        Message = $"No flights found for airline '{airlineName}'."
                    };
                    _cache.Set(cacheKey, new List<int>(), TimeSpan.FromMinutes(10));
                    return NotFound(emptyResult);
                }

                var response = matchingIds
                    .Select(fid => new
                    {
                        FlightId = fid,
                        DetailUrl = Url.Action(
                            action: nameof(GetByFlightId),
                            controller: "FlightExports",
                            values: new { flightId = fid }
                        )
                    })
                    .ToList();

                _cache.Set(cacheKey, matchingIds, TimeSpan.FromMinutes(10));
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving flights by airline full name: {ex.Message}");
            }
        }

        [HttpGet("by-airport/{airportName}")]
        public async Task<IActionResult> GetByAirport(string airportName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(airportName))
                    return BadRequest(new { Message = "Airport name is required." });

                airportName = airportName.Trim().ToUpper();
                string cacheKey = $"FlightsByAirport_{airportName}";

                if (_cache.TryGetValue(cacheKey, out List<int> cachedIds))
                {
                    var cachedResponse = cachedIds
                        .Select(fid => new
                        {
                            FlightId = fid,
                            DetailUrl = Url.Action(
                                action: nameof(GetByFlightId),
                                controller: "FlightExports",
                                values: new { flightId = fid }
                            )
                        })
                        .ToList();

                    return Ok(cachedResponse);
                }

                var matchingIds = await _context.FlightExportInfos
                    .AsNoTracking()
                    .Where(f => f.Airport != null && f.Airport.ToUpper() == airportName)
                    .OrderBy(f => f.FlightId)
                    .Take(100)
                    .Select(f => f.FlightId)
                    .ToListAsync();

                if (!matchingIds.Any())
                {
                    var emptyResult = new
                    {
                        Message = $"No flights found for airport '{airportName}'."
                    };
                    _cache.Set(cacheKey, new List<int>(), TimeSpan.FromMinutes(10));
                    return NotFound(emptyResult);
                }

                var response = matchingIds
                    .Select(fid => new
                    {
                        FlightId = fid,
                        DetailUrl = Url.Action(
                            action: nameof(GetByFlightId),
                            controller: "FlightExports",
                            values: new { flightId = fid }
                        )
                    })
                    .ToList();

                _cache.Set(cacheKey, matchingIds, TimeSpan.FromMinutes(10));
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving flights by airport: {ex.Message}");
            }
        }

        [HttpGet("flight-statistics")]
        public async Task<IActionResult> GetFlightStatistics(string startDatetime = null, string endDatetime = null)
        {
            try
            {
                DateTime? parsedStart = null;
                DateTime? parsedEnd = null;

                if (!string.IsNullOrWhiteSpace(startDatetime))
                {
                    if (!DateTime.TryParse(startDatetime, out var tempStart))
                        return BadRequest(new { Message = "Invalid Start Date format." });
                    parsedStart = tempStart;
                }

                if (!string.IsNullOrWhiteSpace(endDatetime))
                {
                    if (!DateTime.TryParse(endDatetime, out var tempEnd))
                        return BadRequest(new { Message = "Invalid End Date format." });
                    parsedEnd = tempEnd;
                }

                if (parsedStart != null && parsedEnd != null && parsedStart > parsedEnd)
                    return BadRequest(new { Message = "Start Date must be before End Date." });

                string keyStart = parsedStart?.ToString("yyyyMMddHHmmss") ?? "null";
                string keyEnd = parsedEnd?.ToString("yyyyMMddHHmmss") ?? "null";
                string cacheKey = $"FlightStats_{keyStart}_{keyEnd}";

                if (_cache.TryGetValue(cacheKey, out object cachedStats))
                    return Ok(cachedStats);

                var baseQuery = _context.FlightExportInfos.AsNoTracking().AsQueryable();

                if (parsedStart != null)
                    baseQuery = baseQuery.Where(f => f.ScheduledLocal >= parsedStart.Value);

                if (parsedEnd != null)
                    baseQuery = baseQuery.Where(f => f.ScheduledLocal <= parsedEnd.Value);

                var delayedCount = await baseQuery
                    .CountAsync(f => f.ScheduledLocal != null &&
                                     f.ActualLocal != null &&
                                     f.ActualLocal > f.ScheduledLocal);

                var arrivingCount = await baseQuery
                    .CountAsync(f => f.TrafficType != null &&
                                     f.TrafficType.ToUpper() == "A");

                var divertedCount = await baseQuery
                    .CountAsync(f => f.Diverted != null && f.Diverted);

                var nachtvluchtCount = await baseQuery
                    .CountAsync(f => f.Nachtvlucht != null && f.Nachtvlucht);

                var popularAirports = await baseQuery
                    .Where(f => f.Airport != null)
                    .GroupBy(f => f.Airport)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => new
                    {
                        Airport = g.Key,
                        FlightCount = g.Count()
                    })
                    .ToListAsync();

                var totalSeats = await baseQuery.SumAsync(f => f.Seats ?? 0);

                var euFlights = await baseQuery.CountAsync(f => f.Eu == true);
                var nonEuFlights = await baseQuery.CountAsync(f => f.Eu == false);

                var paxMale = await baseQuery.SumAsync(f => f.PaxMale ?? 0);
                var paxFemale = await baseQuery.SumAsync(f => f.PaxFemale ?? 0);
                var paxInfant = await baseQuery.SumAsync(f => f.PaxInfant ?? 0);
                var paxChild = await baseQuery.SumAsync(f => f.PaxChild ?? 0);
                var paxTotal = await baseQuery.SumAsync(f => f.TotaalPax ?? 0);
                var paxTerminal = await baseQuery.SumAsync(f => f.TerminalPax ?? 0);
                var paxTransit = await baseQuery.SumAsync(f => f.TransitPax ?? 0);

                var bagTotal = await baseQuery.SumAsync(f => f.TotaalBagsWeight ?? 0);
                var bagTerminal = await baseQuery.SumAsync(f => f.TerminalBagsWeight ?? 0);
                var bagTransit = await baseQuery.SumAsync(f => f.TransitBagsWeight ?? 0);
                var bagCountTotal = await baseQuery.SumAsync(f => f.TotaalBags ?? 0);
                var bagCountTerminal = await baseQuery.SumAsync(f => f.TerminalBags ?? 0);
                var bagCountTransit = await baseQuery.SumAsync(f => f.TransitBags ?? 0);

                var totalFlights = await baseQuery.CountAsync();

                var result = new
                {
                    StartDate = parsedStart,
                    EndDate = parsedEnd,
                    DelayedFlights = delayedCount,
                    ArrivingFlights = arrivingCount,
                    DivertedFlights = divertedCount,
                    NachtvluchtFlights = nachtvluchtCount,
                    MostPopularAirports = popularAirports,
                    TotalSeats = totalSeats,
                    EuFlights = euFlights,
                    NonEuFlights = nonEuFlights,
                    PaxStatistics = new
                    {
                        Male = paxMale,
                        Female = paxFemale,
                        Infant = paxInfant,
                        Child = paxChild,
                        Total = paxTotal,
                        Terminal = paxTerminal,
                        Transit = paxTransit
                    },
                    BaggageStatistics = new
                    {
                        TotalWeight = bagTotal,
                        TerminalWeight = bagTerminal,
                        TransitWeight = bagTransit,
                        TotalBags = bagCountTotal,
                        TerminalBags = bagCountTerminal,
                        TransitBags = bagCountTransit
                    },
                    TotalFlights = totalFlights
                };

                _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving flight statistics: {ex.Message}");
            }
        }
    }
}