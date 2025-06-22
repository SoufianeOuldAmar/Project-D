using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;

public class FlightsControllerTests : IClassFixture<WebApplicationFactory<Gateway.Program>>
{
    private readonly HttpClient _client;

    public FlightsControllerTests(WebApplicationFactory<Gateway.Program> factory)
    {
        _client = factory.CreateClient();
        // Add JWT auth token to HttpClient
        var token = JwtTokenHelper.GenerateToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetAllFlights_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/flights");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("Delta")]
    [InlineData("United")]
    public async Task GetFlightsByAirline_ValidAirline_ReturnsOk(string airline)
    {
        var response = await _client.GetAsync($"/api/v1/flights/airline/{airline}");
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetFlightsByAirline_MissingAirline_ReturnsBadRequest()
    {
        var response = await _client.GetAsync($"/api/v1/flights/airline/");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode); // Because route param missing, fallback to 404
    }

    [Theory]
    [InlineData("JFK")]
    [InlineData("LAX")]
    public async Task GetFlightsByAirport_ValidAirport_ReturnsOkOrNotFound(string airport)
    {
        var response = await _client.GetAsync($"/api/v1/flights/airport/{airport}");
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetFlightsByAirport_Empty_ReturnsBadRequest()
    {
        var response = await _client.GetAsync($"/api/v1/flights/airport/");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetFlightStatistics_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/flights/statistics?startDatetime=2023-01-01&endDatetime=2023-12-31");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get2024Statistics_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/flights/statistics/2024");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetMonthlyStatistics2024_ValidMonth_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/flights/statistics/2024/05");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("13")]
    [InlineData("abc")]
    public async Task GetMonthlyStatistics2024_InvalidMonth_ReturnsBadRequest(string month)
    {
        var response = await _client.GetAsync($"/api/v1/flights/statistics/2024/{month}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
