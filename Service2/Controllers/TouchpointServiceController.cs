using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service2.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using ZLinq;
using Microsoft.Extensions.Caching.Memory;

namespace TouchpointsService.Controllers.V1
{
    [Route("api/v1/flight-touchpoints")]
    [ApiController]
    public class FlightTouchpointsController : ControllerBase
    {
        private readonly FlightTouchpointDbContext _context;
        private readonly IMemoryCache _cache;

        public FlightTouchpointsController(FlightTouchpointDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet("yearly-stats")]
        public async Task<IActionResult> GetYearlyStats(int year)
        {
            try
            {

                if (year < 2000 || year > DateTime.Now.Year + 1)
                    return BadRequest($"Invalid year. Choose between 2000 and {DateTime.Now.Year + 1}.");

                string cacheKey = $"YearlyStats_{year}";
                if (_cache.TryGetValue(cacheKey, out object cached))
                    return Ok(cached);


                var yearlyFlights = await _context.FlightTouchpointInfos
                    .AsNoTracking()
                    .Where(f => f.TouchpointTime.Year == year)
                    .Select(f => new
                    {
                        f.FlightId,
                        f.TouchpointPax,
                        f.ActualLocal,
                        f.ScheduledLocal,
                        f.Airport,
                        f.AircraftType
                    })
                    .ToListAsync();


                if (!yearlyFlights.Any())
                {
                    var emptyResult = new
                    {
                        Year = year,
                        Message = "No data found for this year."
                    };
                    _cache.Set(cacheKey, emptyResult, TimeSpan.FromMinutes(10));
                    return Ok(emptyResult);
                }

                var dataEnumerable = yearlyFlights.AsValueEnumerable();

                var flightCount = dataEnumerable
                    .Select(f => f.FlightId)
                    .Distinct()
                    .Count();

                var totalPassengers = dataEnumerable
                    .Sum(f => f.TouchpointPax);

                var averageDelay = dataEnumerable
                    .Where(f => f.ActualLocal > f.ScheduledLocal)
                    .Select(f => (f.ActualLocal - f.ScheduledLocal).TotalMinutes)
                    .Average();

                var topAirports = dataEnumerable
                    .GroupBy(f => f.Airport)
                    .OrderByDescending(g => g.Count())
                    .Take(3)
                    .Select(g => new
                    {
                        Airport = g.Key,
                        Count = g.Count()
                    })
                    .ToList();

                var stats = new
                {
                    Year = year,
                    FligtCount = flightCount,
                    TotalPassengers = totalPassengers,
                    AverageDelay = averageDelay,
                    TopAirports = topAirports
                };

                _cache.Set(cacheKey, stats, TimeSpan.FromMinutes(10));
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving yearly stats: {ex.Message}");
            }
        }


        [HttpGet("monthly-stats")]
        public async Task<IActionResult> GetMonthlyStats(int year, int month)
        {
            try
            {
                if (month < 1 || month > 12)
                    return BadRequest("Invalid month. Choose between 1 and 12.");

                string cacheKey = $"MonthlyStats_{year}_{month}";
                if (_cache.TryGetValue(cacheKey, out object cachedMonthly))
                    return Ok(cachedMonthly);

                var monthlyFlights = await _context.FlightTouchpointInfos
                    .AsNoTracking()
                    .Where(f => f.TouchpointTime.Year == year && f.TouchpointTime.Month == month)
                    .Select(f => new
                    {
                        f.FlightId,
                        f.TouchpointTime,
                        f.TouchpointPax,
                        f.AircraftType
                    })
                    .ToListAsync();

                if (!monthlyFlights.Any())
                {
                    var emptyResult = new
                    {
                        Year = year,
                        Month = DateTimeFormatInfo.InvariantInfo.GetMonthName(month),
                        FlightCount = 0,
                        TotalPassengers = (int?)null,
                        BusiestDay = 0,
                        TopAircraftTypes = Enumerable.Empty<string>(),
                        Message = "No data found."
                    };
                    _cache.Set(cacheKey, emptyResult, TimeSpan.FromMinutes(10));
                    return Ok(emptyResult);
                }

                var monthlyEnumerable = monthlyFlights.AsValueEnumerable();

                var flightCount = monthlyEnumerable
                    .Select(f => f.FlightId)
                    .Distinct()
                    .Count();

                var totalPassengers = monthlyEnumerable
                    .Sum(f => f.TouchpointPax);

                var busiestDay = monthlyEnumerable
                    .GroupBy(f => f.TouchpointTime.Day)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault();

                var topAircraftTypes = monthlyEnumerable
                    .GroupBy(f => f.AircraftType)
                    .OrderByDescending(g => g.Count())
                    .Take(3)
                    .Select(g => g.Key)
                    .ToList();


                var stats = new
                {
                    Year = year,
                    Month = DateTimeFormatInfo.InvariantInfo.GetMonthName(month),
                    FlightCount = flightCount,
                    TotalPassengers = (int?)totalPassengers,
                    BusiestDay = busiestDay,
                    TopAircraftTypes = topAircraftTypes,
                    Message = (string)null
                };

                _cache.Set(cacheKey, stats, TimeSpan.FromMinutes(10));
                return Ok(stats);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Error retreving monthly stats: {ex.Message}");
            }
        }

        [HttpGet("busy-hours-per-day")]
        public async Task<IActionResult> GetBusyHoursPerDay(int timeWindowMinutes = 60)
        {
            try
            {
                string cacheKey = $"BusyHoursPerDay_{timeWindowMinutes}";
                if (_cache.TryGetValue(cacheKey, out object cachedBusyHours))
                    return Ok(cachedBusyHours);

                var allTouchpoints = await _context.FlightTouchpointInfos
                    .AsNoTracking()
                    .ToListAsync();

                var allTouchpointEnumerable = allTouchpoints.AsValueEnumerable();


                var result = allTouchpointEnumerable
                    .GroupBy(f => f.TouchpointTime.DayOfWeek)
                    .Select(g => new
                    {
                        DayOfWeek = g.Key.ToString(),
                        BusiestPeriod = g
                            .GroupBy(f => new
                            {
                                Hour = f.TouchpointTime.Hour,
                                TimeBlock = f.TouchpointTime.Minute / timeWindowMinutes
                            })
                            .Select(t => new
                            {
                                StartTime = new DateTime(
                                    2000, 1, 1,
                                    t.Key.Hour,
                                    t.Key.TimeBlock * timeWindowMinutes,
                                    0
                                ),
                                Count = t.Count()
                            })
                            .OrderByDescending(t => t.Count)
                            .FirstOrDefault()
                    })
                    .OrderBy(d => d.DayOfWeek)
                    .ToList();

                var response = result
                    .AsValueEnumerable()
                    .Select(d => new
                    {
                        Day = d.DayOfWeek,
                        BusiestPeriod = d.BusiestPeriod != null
                            ? $"{d.BusiestPeriod.StartTime:HH:mm} - {d.BusiestPeriod.StartTime.AddMinutes(timeWindowMinutes):HH:mm}"
                            : "No data.",
                        TouchpointCount = d.BusiestPeriod?.Count ?? 0
                    })
                    .ToList();

                _cache.Set(cacheKey, response, TimeSpan.FromMinutes(10));
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fout bij ophalen drukste uren: {ex.Message}");
            }
        }

        [HttpGet("all-touchpoints-with-ids")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllTouchpointsWithIds()
        {
            try
            {
                string cacheKey = "AllTouchpointsWithIds";
                if (_cache.TryGetValue(cacheKey, out object cachedLinks))
                    return Ok(cachedLinks);

                var allTouchpoints = await _context.FlightTouchpointInfos
                    .AsNoTracking()
                    .ToListAsync();

                var allTouchpointEnumerable = allTouchpoints.AsValueEnumerable();

                var links = allTouchpointEnumerable
                    .Select(r =>
                    {
                        var id = ComputeHash(r);
                        return new
                        {
                            id,
                            Touchpoint = r.Touchpoint,
                            Url = Url.Action(
                                action: nameof(GetByTouchpointId),
                                controller: "FlightTouchpoints",
                                values: new { id }
                            )
                        };
                    })
                    .ToList();

                _cache.Set(cacheKey, links, TimeSpan.FromMinutes(10));
                return Ok(links);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving all touchpoint: {ex.Message}");
            }
        }

        [HttpGet("by-touchpoint-id/{id}")]
        public async Task<ActionResult<FlightTouchpointInfo>> GetByTouchpointId(string id)
        {
            try
            {
                string cacheKey = $"Touchpoint_{id}";
                if (_cache.TryGetValue(cacheKey, out FlightTouchpointInfo cachedRecord))
                    return Ok(cachedRecord);

                var allTouchpoints = await _context.FlightTouchpointInfos
                    .AsNoTracking()
                    .ToListAsync();

                var allTouchpointEnumerable = allTouchpoints.AsValueEnumerable();

                var match = allTouchpointEnumerable
                    .FirstOrDefault(r => ComputeHash(r) == id);

                if (match == null)
                    return NotFound();

                _cache.Set(cacheKey, match, TimeSpan.FromMinutes(10));
                return Ok(match);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving touchpoint entry: {ex.Message}");
            }
        }

        [HttpGet("most-common-traffic-type")]
        public async Task<IActionResult> GetMostCommonTrafficType()
        {
            try
            {
                const string cacheKey = "MostCommonTrafficType";
                if (_cache.TryGetValue(cacheKey, out object cachedStats))
                    return Ok(cachedStats);

                var trafficCounts = await _context.FlightTouchpointInfos
                    .AsNoTracking()
                    .GroupBy(f => f.TrafficType)
                    .Select(g => new
                    {
                        TrafficType = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .ToListAsync();

                if (!trafficCounts.Any())
                    return NotFound("No traffic type data available");

                var totalCount = trafficCounts.Sum(x => x.Count);

                var trafficStats = trafficCounts
                    .Select(x => new
                    {
                        TrafficType = x.TrafficType,
                        Count = x.Count,
                        Percentage = totalCount > 0
                            ? (double)x.Count / totalCount * 100
                            : 0
                    })
                    .ToList();

                var stats = new
                {
                    MostCommonType = trafficStats.First(),
                    AllTypes = trafficStats
                };

                _cache.Set(cacheKey, stats, TimeSpan.FromMinutes(10));
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving traffic data: {ex.Message}");
            }
        }

        [HttpGet("flights-per-airport")]
        public async Task<IActionResult> GetFlightsPerAirport()
        {
            try
            {
                string cacheKey = "FlightsPerAirport";
                if (_cache.TryGetValue(cacheKey, out object cachedStats))
                    return Ok(cachedStats);

                var stats = await _context.FlightTouchpointInfos
                    .AsNoTracking()
                    .Select(f => new { f.Airport, f.FlightId })
                    .Distinct()
                    .GroupBy(f => f.Airport)
                    .Select(g => new
                    {
                        Airport = g.Key,
                        FlightCount = g.Count()
                    })
                    .ToListAsync();

                _cache.Set(cacheKey, stats, TimeSpan.FromMinutes(10));
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving flights per airport: {ex.Message}");
            }
        }

        [HttpGet("countries-with-most-flights")]
        public async Task<IActionResult> GetCountriesWithMostFlights(int top = 10)
        {
            try
            {
                string cacheKey = $"TopCountries_{top}";
                if (_cache.TryGetValue(cacheKey, out object cachedStats))
                    return Ok(cachedStats);

                var stats = await _context.FlightTouchpointInfos
                    .AsNoTracking()
                    .GroupBy(f => f.Country)
                    .Select(g => new
                    {
                        Country = g.Key,
                        FlightCount = g.Select(f => f.FlightId).Distinct().Count(),
                        MostPopularAirport = g
                            .GroupBy(f => f.Airport)
                            .OrderByDescending(a => a.Count())
                            .Select(a => a.Key)
                            .FirstOrDefault()
                    })
                    .OrderByDescending(x => x.FlightCount)
                    .Take(top)
                    .ToListAsync();

                _cache.Set(cacheKey, stats, TimeSpan.FromMinutes(10));
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving top countries: {ex.Message}");
            }
        }

        private static string ComputeHash(FlightTouchpointInfo r)
        {
            var sb = new StringBuilder()
                .Append(r.FlightId).Append('|')
                .Append(r.TimetableId).Append('|')
                .Append(r.FlightNumber).Append('|')
                .Append(r.AirlineShortname).Append('|')
                .Append(r.Airport).Append('|')
                .Append(r.Country).Append('|')
                .Append(r.Touchpoint).Append('|')
                .Append(r.TouchpointTime.ToString("o")).Append('|')
                .Append(r.ScheduledLocal.ToString("o")).Append('|')
                .Append(r.ActualLocal.ToString("o")).Append('|')
                .Append(r.AircraftType).Append('|')
                .Append(r.TrafficType).Append('|')
                .Append(r.PaxActual);

            using var sha = SHA256.Create();
            var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
            return BitConverter
                .ToString(hashBytes)
                .Replace("-", "")
                .ToLowerInvariant();
        }
    }
}
