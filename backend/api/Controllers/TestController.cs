using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("private")]
        public async Task<IActionResult> Private()
        {
            return Ok("private ok");
        }
        [HttpGet("public")]
        public async Task<IActionResult> Public()
        {
            return Ok("public ok");
        }
    }
}
