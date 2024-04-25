using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiVersionNeutral]
    [Route("{version:apiVersion}/files")]
    public class FilesController : BaseController
    {
        private readonly string _songsPath;
        private readonly string _imagesPath;

        public FilesController()
        {
            _songsPath = Directory.GetCurrentDirectory() + "/Files/Songs/";
            _imagesPath = Directory.GetCurrentDirectory() + "/Files/Images/";

            if (!Directory.Exists(_songsPath))
            {
                Directory.CreateDirectory(_songsPath);
            }

            if (!Directory.Exists(_imagesPath))
            {
                Directory.CreateDirectory(_imagesPath);
            }
        }

        [HttpPost("upload/song")]
        public async Task<IActionResult> UploadSongFile(IFormFile song)
        {
            if (song != null)
            {
                string path = _songsPath + song.FileName;

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
                string path = _imagesPath + image.FileName;

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
            byte[] content = await System.IO.File.ReadAllBytesAsync(_songsPath + path);

            return File(content, "audio/*", path);
        }

        [HttpGet("get/image")]
        public async Task<IActionResult> GetImageFile(string path)
        {
            byte[] content = await System.IO.File.ReadAllBytesAsync(_imagesPath + path);

            return File(content, "audio/*", path);
        }

    }
}
