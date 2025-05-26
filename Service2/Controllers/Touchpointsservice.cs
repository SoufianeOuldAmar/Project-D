using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service2.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;


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

        [HttpGet("stats per jaar")]
        public async Task<IActionResult> GetYearlyStats(int year)
        {
            try
            {

                if (year < 2000 || year > DateTime.Now.Year + 1)
                    return BadRequest($"Ongeldig jaar. Kies tussen 2000 en {DateTime.Now.Year + 1}");


                var rawData = await _context.FlightTouchpointInfos
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


                if (!rawData.Any())
                    return Ok(new { Jaar = year, Bericht = "Geen data gevonden voor dit jaar" });

                var stats = new
                {
                    Jaar = year,
                    AantalVluchten = rawData.Select(f => f.FlightId).Distinct().Count(),
                    TotaalPassagiers = rawData.Sum(f => f.TouchpointPax),
                    GemVertraging = rawData
                        .Where(f => f.ActualLocal > f.ScheduledLocal)
                        .Average(f => (f.ActualLocal - f.ScheduledLocal).TotalMinutes),
                    TopLuchthavens = rawData
                        .GroupBy(f => f.Airport)
                        .OrderByDescending(g => g.Count())
                        .Take(3)
                        .Select(g => new
                        {
                            Luchthaven = g.Key,
                            Aantal = g.Count()
                        })
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fout bij ophalen jaaroverzicht: {ex.Message}");
            }
        }


        [HttpGet("stats per maand")]
        public async Task<IActionResult> GetMonthlyStats(int year, int month)
        {
            try
            {
                if (month < 1 || month > 12)
                    return BadRequest("Ongeldige maand. Kies tussen 1-12");


                var monthlyFlights = await _context.FlightTouchpointInfos
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
                    return Ok(new
                    {
                        Jaar = year,
                        Maand = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                        AantalVluchten = 0,
                        TotaalPassagiers = (int?)null,
                        DruksteDag = 0,
                        TopVliegtuigTypes = Enumerable.Empty<string>(),
                        Bericht = "Geen data gevonden"
                    });
                }


                var aantalVluchten = monthlyFlights
                    .Select(f => f.FlightId)
                    .Distinct()
                    .Count();

                var totaalPassagiers = monthlyFlights
                    .Sum(f => f.TouchpointPax);

                var druksteDag = monthlyFlights
                    .GroupBy(f => f.TouchpointTime.Day)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault();

                var topVliegtuigTypes = monthlyFlights
                    .GroupBy(f => f.AircraftType)
                    .OrderByDescending(g => g.Count())
                    .Take(3)
                    .Select(g => g.Key)
                    .ToList();


                var stats = new
                {
                    Jaar = year,
                    Maand = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    AantalVluchten = aantalVluchten,
                    TotaalPassagiers = (int?)totaalPassagiers,
                    DruksteDag = druksteDag,
                    TopVliegtuigTypes = topVliegtuigTypes,
                    Bericht = (string)null
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Fout bij ophalen maandoverzicht: {ex.Message}");
            }
        }

        [HttpGet("DruksteUrenPerDag")]
        public async Task<IActionResult> GetDruksteUrenPerDag(
   [FromQuery] int timeWindowMinutes = 60)
        {
            try
            {
                _context.Database.SetCommandTimeout(300);


                var allTouchpoints = await _context.FlightTouchpointInfos
                    .ToListAsync();


                var result = allTouchpoints
                    .GroupBy(f => f.TouchpointTime.DayOfWeek)
                    .Select(g => new
                    {
                        DagVanDeWeek = g.Key.ToString(),
                        DrukstePeriode = g
                            .GroupBy(f => new
                            {
                                Uur = f.TouchpointTime.Hour,
                                TijdBlok = f.TouchpointTime.Minute / timeWindowMinutes
                            })
                            .Select(t => new
                            {
                                StartTijd = new DateTime(2000, 1, 1, t.Key.Uur, t.Key.TijdBlok * timeWindowMinutes, 0),
                                Aantal = t.Count()
                            })
                            .OrderByDescending(t => t.Aantal)
                            .FirstOrDefault()
                    })
                    .OrderBy(d => d.DagVanDeWeek)
                    .ToList();

                var response = result.Select(d => new
                {
                    Dag = d.DagVanDeWeek,
                    DrukstePeriode = d.DrukstePeriode != null ?
                        $"{d.DrukstePeriode.StartTijd:HH:mm} - {d.DrukstePeriode.StartTijd.AddMinutes(timeWindowMinutes):HH:mm}" :
                        "Geen data",
                    AantalTouchpoints = d.DrukstePeriode?.Aantal ?? 0
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fout bij ophalen drukste uren: {ex.Message}");
            }
        }

        [HttpGet("touchpoints", Name = nameof(GetAllTouchpointLinks))]
        public async Task<ActionResult<IEnumerable<object>>> GetAllTouchpointLinks()
        {
            var allRecords = await _context.FlightTouchpointInfos.ToListAsync();

            var links = allRecords.Select(r =>
            {
                var id = ComputeHash(r);
                return new
                {
                    id,
                    touchpoint = r.Touchpoint,
                    url = Url.Link(
                        nameof(GetTouchpointById),
                        new { id }
                    )
                };
            });

            return Ok(links);
        }

        [HttpGet("touchpoints/{id}", Name = nameof(GetTouchpointById))]
        public async Task<ActionResult<FlightTouchpointInfo>> GetTouchpointById(string id)
        {
            // load all into memory and find the one with matching hash
            var allRecords = await _context.FlightTouchpointInfos.ToListAsync();
            var match = allRecords.FirstOrDefault(r => ComputeHash(r) == id);

            if (match == null)
                return NotFound();

            return Ok(match);
        }

        [HttpGet("stats/most-common-traffic-type")]
        public async Task<IActionResult> GetMostCommonTrafficType()
        {
            try
            {

                var trafficStats = await _context.FlightTouchpointInfos
                    .GroupBy(f => f.TrafficType)
                    .Select(g => new
                    {
                        TrafficType = g.Key,
                        Count = g.Count(),
                        Percentage = (double)g.Count() / _context.FlightTouchpointInfos.Count() * 100
                    })
                    .OrderByDescending(x => x.Count)
                    .ToListAsync();

                if (!trafficStats.Any())
                    return NotFound("No traffic type data available");

                return Ok(new
                {
                    MostCommonType = trafficStats.First(),
                    AllTypes = trafficStats
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving traffic data: {ex.Message}");
            }
        }

        [HttpGet("stats/flights-per-airport")]
        public async Task<IActionResult> GetFlightsPerAirport()
        {

            _context.Database.SetCommandTimeout(300);

            var stats = await _context.FlightTouchpointInfos
                .Select(f => new { f.Airport, f.FlightId })
                .Distinct()
                .GroupBy(f => f.Airport)
                .Select(g => new
                {
                    Airport = g.Key,
                    FlightCount = g.Count()
                })
                .ToListAsync();

            return Ok(stats);
        }

        [HttpGet("stats/landen-met-meeste-vluchten")]
        public async Task<IActionResult> GetLandenMetMeesteVluchten([FromQuery] int top = 10)
        {
            var stats = await _context.FlightTouchpointInfos
                .GroupBy(f => f.Country)
                .Select(g => new
                {
                    Land = g.Key,
                    AantalVluchten = g.Select(f => f.FlightId).Distinct().Count(),
                    PopulairsteLuchthaven = g
                        .GroupBy(f => f.Airport)
                        .OrderByDescending(a => a.Count())
                        .Select(a => a.Key)
                        .FirstOrDefault()
                })
                .OrderByDescending(x => x.AantalVluchten)
                .Take(top)
                .ToListAsync();

            return Ok(stats);
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
