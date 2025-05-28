using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace ApiGateway.Controllers.V1{
    [ApiController]
    [Route("api/v1/flights")]
    public class FlightsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public readonly string BaseUrl = "http://localhost:5041/api/v1/flight-exports";
        public FlightsController()
        {
            _httpClient = new HttpClient();
        }

        [HttpGet("all-flights")]
    public async Task<IActionResult> GetAllFlights()
    {
        try
        {
            var url = $"{BaseUrl}/all-flights";  // Match case with your endpoint
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                // Forward the error response from the underlying service
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

        [HttpGet("by-airline")]
        public async Task<IActionResult> GetFlightsByAirline([FromQuery] string airline)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(airline))
                    return BadRequest(new { message = "Airline parameter is required" });

                var url = $"{BaseUrl}/by-airline?airline={airline}";
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

        // New endpoint for airport filtering
        [HttpGet("by-airport")]
        public async Task<IActionResult> GetFlightsByAirport([FromQuery] string airportCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(airportCode))
                    return BadRequest(new { message = "Airport code parameter is required" });

                var url = $"{BaseUrl}/by-airport?airportCode={airportCode}";
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

        [HttpGet("flight-statistics")]
        public async Task<IActionResult> GetFlightStatistics(
        [FromQuery] string startDatetime = null,
        [FromQuery] string endDatetime = null)
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

        [HttpGet("Statistics2024")]
        public async Task<IActionResult> statistics()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/flight-statistics?startDatetime=2024-01-01&endDatetime=2024-12-31");
            return await CreateResponse(response);
        }

        [HttpGet("monthly-statistics-2024")]
        public async Task<IActionResult> GetMonthlyStatistics2024([FromQuery] string month)
        {
            // Validate month parameter
            if (string.IsNullOrWhiteSpace(month))
                return BadRequest(new { message = "Month parameter is required (format: MM)." });

            if (!int.TryParse(month, out int monthNumber) && monthNumber < 1 || monthNumber > 12)
                return BadRequest(new { message = "Invalid month. Please provide a value between 01 and 12." });

            try
            {
                // Create date range for the entire month
                var startDate = new DateTime(2024, monthNumber, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1); // Last day of the month

                // Call the statistics endpoint
                var url = $"{BaseUrl}/flight-statistics" +
                        $"?startDatetime={startDate:yyyy-MM-dd}&endDatetime={endDate:yyyy-MM-dd}";

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

        public static async Task<IActionResult> CreateResponse(HttpResponseMessage response)
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
    public class TouchpointsGatewayController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5153/api/v1/touchpoints";

        public TouchpointsGatewayController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet("YearlyStats")]
        public async Task<IActionResult> GetYearlyStats([FromQuery] int year = 2024)
        {
            var url = $"{BaseUrl}/YearlyStats?year={year}";
            return await ForwardRequest(url);
        }

        [HttpGet("MonthlyStats")]
        public async Task<IActionResult> GetMonthlyStats([FromQuery] int year = 2024, [FromQuery] int month = 01)
        {
            var url = $"{BaseUrl}/MonthlyStats?year={year}&month={month}";
            return await ForwardRequest(url);
        }

        [HttpGet("BusyHoursPerDay")]
        public async Task<IActionResult> GetBusyHoursPerDay([FromQuery] int timeWindowMinutes = 60)
        {
            var url = $"{BaseUrl}/BusyHoursPerDay?timeWindowMinutes={timeWindowMinutes}";
            return await ForwardRequest(url);
        }

        [HttpGet("touchpoints")]
        public async Task<IActionResult> GetAllTouchpointLinks()
        {
            var url = $"{BaseUrl}/touchpoints";
            return await ForwardRequest(url);
        }

        [HttpGet("touchpoints/{id}")]
        public async Task<IActionResult> GetTouchpointById(string id)
        {
            var url = $"{BaseUrl}/touchpoints/{id}";
            return await ForwardRequest(url);
        }

        [HttpGet("stats/most-common-traffic-type")]
        public async Task<IActionResult> GetMostCommonTrafficType()
        {
            var url = $"{BaseUrl}/stats/most-common-traffic-type";
            return await ForwardRequest(url);
        }

        [HttpGet("stats/flights-per-airport")]
        public async Task<IActionResult> GetFlightsPerAirport()
        {
            var url = $"{BaseUrl}/stats/flights-per-airport";
            return await ForwardRequest(url);
        }

        [HttpGet("stats/landen-met-meeste-vluchten")]
        public async Task<IActionResult> GetLandenMetMeesteVluchten([FromQuery] int top = 10)
        {
            var url = $"{BaseUrl}/stats/landen-met-meeste-vluchten?top={top}";
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
