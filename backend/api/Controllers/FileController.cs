using System.Security.Claims;
using api.Dtos;
using api.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IMediaUploadService _uploadService;
        private readonly IAiService _aiService;
        public FileController(IMediaUploadService mediaUploadService,IAiService aiService)
        {
            _uploadService=mediaUploadService;
            _aiService=aiService;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromBody] UploadFileDto Dto,CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId==null || String.IsNullOrWhiteSpace(userId))
            {
                return NotFound("Not logged");
            }
            Guid userIdGuid = Guid.Parse(userId);
            var serviceResult = await _uploadService.GenerateUrl(Dto,userIdGuid,ct);
            return Ok(serviceResult);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("upload-done/{fileId:guid}")]
        public async Task<IActionResult> UploadFileDone(Guid fileId,CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId==null || String.IsNullOrWhiteSpace(userId))
            {
                return NotFound("Not logged");
            }
            var serviceResult = await _uploadService.UploadDone(fileId,ct);
            if (serviceResult.Success == true)
            {
                await _aiService.AddFile(serviceResult.Data!,fileId.ToString(),userId);
            }
            return Ok(serviceResult);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("search")]
        public async Task<IActionResult> UploadFileDone([FromQuery] string queryParam,CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId==null || String.IsNullOrWhiteSpace(userId))
            {
                return NotFound("Not logged");
            }
            var serviceResult = await _aiService.SearchFiles(queryParam,userId.ToString());
            
            return Ok(serviceResult);
        }
    }
}
