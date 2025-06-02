using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gateway.Entities;
using Gateway.Models;
using Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Gateway.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [Authorize]
    public class AuthController(IAuthService authService) : ControllerBase
    {

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user = await authService.RegisterAsync(request);
            if (user is null)
            {
                return BadRequest("Username already exists");
            }

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
        {
            var result = await authService.LoginAsync(request);
            if (result is null)
            {
                return BadRequest("Invalid username or password");
            }

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
            {
                return Unauthorized("Invalid refresh token");
            }

            return Ok(result);
        }

        [HttpGet]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            return Ok("You are authenticated");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You are an Admin");
        }
    }
}
