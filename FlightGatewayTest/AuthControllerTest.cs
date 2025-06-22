using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Gateway.Models;
using System.Net.Http.Json;

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
        // Arrange
        var loginDto = new UserDto
        {
            Username = "existinguser",
            Password = "Password123!"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(loginDto), 
            Encoding.UTF8, 
            "application/json"
        );

        // Act
        var response = await _client.PostAsync("/api/auth/login", content);

        // Assert
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

    // [Fact]
    // public async Task AdminOnlyEndpoint_AsAdmin_ReturnsOk()
    // {
    //     var token = JwtTokenHelper.GenerateToken("adminuser", "Admin");
    //     _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    //     var response = await _client.GetAsync("/api/auth/admin-only");
    //     Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    // }
    [Fact]
    public async Task AdminOnlyEndpoint_LoginAndUseToken_ReturnsOk()
    {
        // 1) Log in as the seeded admin user
        var loginContent = new StringContent(
            JsonSerializer.Serialize(new { Username = "adminuser", Password = "AdminPassword123!" }),
            Encoding.UTF8,
            "application/json"
        );

        var loginResponse = await _client.PostAsync("/api/auth/login", loginContent);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var tokenDto = await loginResponse.Content.ReadFromJsonAsync<TokenResponseDto>();
        Assert.False(string.IsNullOrEmpty(tokenDto?.AccessToken));

        // 2) Use that token to call the admin-only endpoint
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", tokenDto.AccessToken);

        var adminResponse = await _client.GetAsync("/api/auth/admin-only");
        Assert.Equal(HttpStatusCode.OK, adminResponse.StatusCode);
    }


    [Fact]
    public async Task AdminOnlyEndpoint_AsUser_ReturnsForbidden()
    {
        var token = JwtTokenHelper.GenerateToken("existinguser", "User");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/auth/admin-only");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
