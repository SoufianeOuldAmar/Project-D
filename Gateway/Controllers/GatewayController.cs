using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace ApiGateway.Controllers.V1
{
    [ApiController]
    [Route("api/v1/flights")]
    [Authorize]
    public class FlightsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public readonly string BaseUrl = "http://localhost:5041/api/v1/flight-exports";

        public FlightsController()
        {
            _httpClient = new HttpClient();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFlights()
        {
            try
            {
                var url = $"{BaseUrl}/all-flights-with-ids";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                }

                var flights = await response.Content.ReadAsStringAsync();
                return Ok(flights);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Failed to retrieve flights: {ex.Message}");
            }
        }

        [HttpGet("airline/{airline}")]
        public async Task<IActionResult> GetFlightsByAirline(string airline)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(airline))
                    return BadRequest(new { message = "Airline parameter is required" });

                var url = $"{BaseUrl}/by-airline-full-name/{airline}";
                var response = await _httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound(await response.Content.ReadAsStringAsync());

                response.EnsureSuccessStatusCode();
                return Ok(await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error retrieving flights by airline: {ex.Message}");
            }
        }

        [HttpGet("airport/{airportName}")]
        public async Task<IActionResult> GetFlightsByAirport(string airportName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(airportName))
                    return BadRequest(new { message = "Airport code parameter is required" });

                var url = $"{BaseUrl}/by-airport/{airportName}";
                var response = await _httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound(await response.Content.ReadAsStringAsync());

                response.EnsureSuccessStatusCode();
                return Ok(await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error retrieving flights by airport: {ex.Message}");
            }
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetFlightStatistics(string startDatetime = null, string endDatetime = null)
        {
            try
            {
                var url = $"{BaseUrl}/flight-statistics?startDatetime={startDatetime}&endDatetime={endDatetime}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                return Ok(await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error calling get flight statistics: " + ex.Message);
            }
        }

        [HttpGet("statistics/2024")]
        public async Task<IActionResult> Get2024Statistics()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/flight-statistics?startDatetime=2024-01-01&endDatetime=2024-12-31");
            return await CreateResponse(response);
        }

        [HttpGet("statistics/2024/{month}")]
        public async Task<IActionResult> GetMonthlyStatistics2024(string month)
        {
            if (string.IsNullOrWhiteSpace(month))
                return BadRequest(new { message = "Month parameter is required (format: MM)." });

            if (!int.TryParse(month, out int monthNumber) && monthNumber < 1 || monthNumber > 12)
                return BadRequest(new { message = "Invalid month. Please provide a value between 01 and 12." });

            try
            {
                var startDate = new DateTime(2024, monthNumber, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var url = $"{BaseUrl}/flight-statistics?startDatetime={startDate:yyyy-MM-dd}&endDatetime={endDate:yyyy-MM-dd}";

                var response = await _httpClient.GetAsync(url);
                return await CreateResponse(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = $"Error retrieving statistics for month {month}",
                    details = ex.Message
                });
            }
        }

        private static async Task<IActionResult> CreateResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new ObjectResult(new { message = "Request failed", detail = errorContent }) { StatusCode = (int)response.StatusCode };
            }

            var content = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(content);
        }
    }

    [ApiController]
    [Route("api/v1/touchpoints")]
    [Authorize]
    public class TouchpointsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5153/api/v1/flight-touchpoints";

        public TouchpointsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet("statistics/yearly")]
        public async Task<IActionResult> GetYearlyStats(int year = 2024)
        {
            var url = $"{BaseUrl}/yearly-stats?year={year}";
            return await ForwardRequest(url);
        }

        [HttpGet("statistics/monthly")]
        public async Task<IActionResult> GetMonthlyStats(int year = 2024, int month = 1)
        {
            var url = $"{BaseUrl}/monthly-stats?year={year}&month={month}";
            return await ForwardRequest(url);
        }

        [HttpGet("statistics/busy-hours")]
        public async Task<IActionResult> GetBusyHours(int timeWindowMinutes = 60)
        {
            var url = $"{BaseUrl}/busy-hours-per-day?timeWindowMinutes={timeWindowMinutes}";
            return await ForwardRequest(url);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTouchpoints()
        {
            var url = $"{BaseUrl}/all-touchpoints-with-ids";
            return await ForwardRequest(url);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTouchpointById(string id)
        {
            var url = $"{BaseUrl}/by-touchpoint-id/{id}";
            return await ForwardRequest(url);
        }

        [HttpGet("statistics/traffic-type")]
        public async Task<IActionResult> GetMostCommonTrafficType()
        {
            var url = $"{BaseUrl}/most-common-traffic-type";
            return await ForwardRequest(url);
        }

        [HttpGet("statistics/flights-per-airport")]
        public async Task<IActionResult> GetFlightsPerAirport()
        {
            var url = $"{BaseUrl}/flights-per-airport";
            return await ForwardRequest(url);
        }

        [HttpGet("statistics/top-countries")]
        public async Task<IActionResult> GetCountriesWithMostFlights(int top = 10)
        {
            var url = $"{BaseUrl}/countries-with-most-flights?top={top}";
            return await ForwardRequest(url);
        }

        private async Task<IActionResult> ForwardRequest(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error calling endpoint: {ex.Message}");
            }
        }
    }
}