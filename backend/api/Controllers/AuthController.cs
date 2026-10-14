using api.Dtos;
using api.IDbServices;
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
                return Ok();

            }
                return BadRequest();

        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login()
        {
            throw new NotImplementedException();
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            throw new NotImplementedException();
        }
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            throw new NotImplementedException();
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            throw new NotImplementedException();
        }

    }
}
