using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public static class JwtTokenHelper
{
    private const string TestSecret   = "x8V9NqvE3KYO6EOWlFwY5L7mA4+P1Z3uKZzXx5XfZ9Zu2Q0U74Od3x2eKN6dY1rJ9HCKL1jUCUY6zkG3u64z6g==";
    private const string TestIssuer   = "FlightInfoGateway";
    private const string TestAudience = "RTHA";

    public static string GenerateToken(string username = "testuser", string role = "User")
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, username), // Add this
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
