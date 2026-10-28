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
        private IMediaUploadService _uploadService;
        public FileController(IMediaUploadService mediaUploadService)
        {
            _uploadService=mediaUploadService;
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
            return Ok(serviceResult);
        }
    }
}
