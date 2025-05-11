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


    }
}