using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ValidUser_ReturnsOk()
    {
        var content = new StringContent(JsonSerializer.Serialize(new
        {
            Username = "newuser",
            Password = "Password123!"
        }), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/auth/register", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Register_ExistingUser_ReturnsBadRequest()
    {
        var content = new StringContent(JsonSerializer.Serialize(new
        {
            Username = "existinguser",
            Password = "Password123!"
        }), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/auth/register", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOk()
    {
        var content = new StringContent(JsonSerializer.Serialize(new
        {
            Username = "existinguser",
            Password = "Password123!"
        }), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/auth/login", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_InvalidToken_ReturnsUnauthorized()
    {
        var content = new StringContent(JsonSerializer.Serialize(new
        {
            RefreshToken = "invalidtoken"
        }), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/auth/refresh-token", content);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CheckAuthentication_WithToken_ReturnsOk()
    {
        var token = JwtTokenHelper.GenerateToken("existinguser", "User");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/auth");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AdminOnlyEndpoint_AsAdmin_ReturnsOk()
    {
        var token = JwtTokenHelper.GenerateToken("adminuser", "Admin");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/auth/admin-only");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AdminOnlyEndpoint_AsUser_ReturnsForbidden()
    {
        var token = JwtTokenHelper.GenerateToken("existinguser", "User");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/auth/admin-only");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
