using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace Gateway.Controllers
{
    [ApiController]
    [Route("gateway")]
    public class GatewayController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public GatewayController()
        {
            _httpClient = new HttpClient();
        }

        [HttpGet("flights")]
        public async Task<IActionResult> VluchtenService()
        {
            try
            {
                var url = $"http://localhost:5041/VluchtenService/AllVluchtenExports";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                return Ok(await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error calling test function:" + ex.Message);
            }
        }

        [HttpGet("touchpoints")]
        public async Task<IActionResult> TouchpointService()
        {
            try
            {
                var url = $"http://localhost:5153/TouchpointsService/AlltouchpointsFlights";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                return Ok(await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error calling get flight report with Flight ID link: " + ex.Message);
            }
        }



        [HttpGet("flights-with-touchpoints")]
        public async Task<IActionResult> AllVluchtenTouchpointServive()
        {
            try
            {
                var url = $"http://localhost:5153/TouchpointsService/AllVluchtenTouchpoints";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                return Ok(await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error calling get flight report with Flight ID link: " + ex.Message);
            }
        }

        [HttpGet("flight-statistics")]
        public async Task<IActionResult> GetFlightStatistics(
        [FromQuery] string startDatetime = null,
        [FromQuery] string endDatetime = null)
        {
            try
            {
                var url = $"http://localhost:5041/VluchtenService/flight-statistics?startDatetime={startDatetime}&endDatetime={endDatetime}";

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
            var response = await _httpClient.GetAsync($"http://localhost:5041/VluchtenService/flight-statistics?startDatetime=2024-01-01&endDatetime=2024-12-31");
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
                var url = $"http://localhost:5041/VluchtenService/flight-statistics" +
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
}
