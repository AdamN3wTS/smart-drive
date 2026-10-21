using api.IServices;
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
            
        }
        [HttpPost]
        public async Task<IActionResult> UploadFile(CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
