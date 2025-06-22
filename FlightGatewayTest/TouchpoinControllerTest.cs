using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

public class TouchpointsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TouchpointsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        var token = JwtTokenHelper.GenerateToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetYearlyStats_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/touchpoints/statistics/yearly?year=2024");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetMonthlyStats_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/touchpoints/statistics/monthly?year=2024&month=5");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetBusyHours_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/touchpoints/statistics/busy-hours?timeWindowMinutes=60");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllTouchpoints_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/touchpoints");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetTouchpointById_ReturnsOkOrNotFound()
    {
        var response = await _client.GetAsync("/api/v1/touchpoints/123");
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMostCommonTrafficType_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/touchpoints/statistics/traffic-type");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetFlightsPerAirport_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/touchpoints/statistics/flights-per-airport");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetCountriesWithMostFlights_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/touchpoints/statistics/top-countries?top=5");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
