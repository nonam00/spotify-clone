using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiVersionNeutral]
    [Route("{version:apiVersion}/files")]
    public class FilesController : BaseController
    {
        [HttpPost("upload/song")]
        public async Task<IActionResult> UploadSongFile(IFormFile song)
        {
            if (song != null)
            {
                string path = "/Files/Songs/" + song.FileName;

                using (var fs = new FileStream(path, FileMode.Create))
                {
                    await song.CopyToAsync(fs);
                }
            }

            return Ok();
        }

        [HttpPost("upload/image")]
        public async Task<IActionResult> UploadImageFile(IFormFile image)
        {
            if (image != null)
            {
                string path = "/Files/Images/" + image.FileName;

                using (var fs = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(fs);
                }
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("get/song")]
        public async Task<IActionResult> GetSongFile(string path)
        {
            byte[] content = await System.IO.File.ReadAllBytesAsync("Files/Songs/" + path);

            return File(content, "audio/*", path);
        }

        [HttpGet("get/image")]
        public async Task<IActionResult> GetImageFile(string path)
        {
            byte[] content = await System.IO.File.ReadAllBytesAsync("Files/Images/" + path);

            return File(content, "audio/*", path);
        }

    }
}
