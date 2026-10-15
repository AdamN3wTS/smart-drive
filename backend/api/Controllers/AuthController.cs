using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Claims;
using api.Dtos;
using api.Helpers;
using api.IDbServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService=authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto,CancellationToken ct)
        {
            if(ModelState.IsValid)
            {
                var serviceResult = await _authService.Register(dto,ct);
                if (serviceResult.Success == false) {

                        return BadRequest(serviceResult);
                }
                Response.Cookies.Append("refreshToken",serviceResult.Data!.refreshTokenPlain, new CookieOptions
                {
                   Expires=serviceResult.Data.refreshTokenExpAt,
                   SameSite=SameSiteMode.Lax,
                   HttpOnly=true,
                   Secure=false

                });
                var response = ResponseHelper.GenerateResponse<RegisterResponseDto>(new RegisterResponseDto(){accessToken=serviceResult.Data.accessToken},true,"Register success");
                return Ok(response);

            }
            return BadRequest(ResponseHelper.GenerateResponse<object?>(null,false,"failed"));

        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto,CancellationToken ct)
        {
            if(ModelState.IsValid)
            {
                var serviceResult = await _authService.Login(dto,ct);
                if (serviceResult.Success == false) {

                        return BadRequest(serviceResult);
                }
                Response.Cookies.Append("refreshToken",serviceResult.Data!.refreshTokenPlain, new CookieOptions
                {
                   Expires=serviceResult.Data.refreshTokenExpAt,
                   SameSite=SameSiteMode.Lax,
                   HttpOnly=true,
                   Secure=false

                });
                var response = ResponseHelper.GenerateResponse<LoginResponseDto>(new LoginResponseDto(){accessToken=serviceResult.Data.accessToken},true,"Login success");
                return Ok(response);

            }
            return BadRequest(ResponseHelper.GenerateResponse<object?>(null,false,"failed"));
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var token = Request.Cookies["refreshToken"];
            if (String.IsNullOrWhiteSpace(token))
            {
                var badResponse = ResponseHelper.GenerateResponse<object?>(null,false,"Error: null Token");
                return BadRequest(badResponse);
            }
            var serviceResult = await _authService.Logout(token,ct);
            return Ok(serviceResult);
        }
        [HttpGet("me")]
        public async Task<IActionResult> Me(CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId==null || String.IsNullOrWhiteSpace(userId))
            {
                return NotFound("Not logged");
            }
            var user = await _authService.Me(userId,ct);
            return Ok(user);
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(CancellationToken ct)
        {
            var token = Request.Cookies["refreshToken"];
            if (String.IsNullOrWhiteSpace(token))
            {
                var badResponse = ResponseHelper.GenerateResponse<object?>(null,false,"Error: null Token");
                return BadRequest(badResponse);
            }
            var serviceResult = await _authService.Refresh(token,ct);
            Response.Cookies.Delete("refreshToken",new CookieOptions
            {
                HttpOnly=true,
                Secure=false,
                SameSite=SameSiteMode.Lax
            });
            if (serviceResult.Success == false)
            {
                return Unauthorized(serviceResult);
            }
            Response.Cookies.Append("refreshToken",serviceResult.Data!.refreshTokenPlain, new CookieOptions
                {
                   Expires=serviceResult.Data.refreshTokenExpAt,
                   SameSite=SameSiteMode.Lax,
                   HttpOnly=true,
                   Secure=false

                });
            var response = ResponseHelper.GenerateResponse<LoginResponseDto>(new LoginResponseDto(){accessToken=serviceResult.Data.accessToken},true,"Login success");
            return Ok(response);
        }

    }
}
