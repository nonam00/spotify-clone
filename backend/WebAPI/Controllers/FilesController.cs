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
        public async Task<ActionResult> UploadSongFile(IFormFile song)
        {
            if (song != null)
            {
                string fileName = Guid.NewGuid().ToString() + song.FileName;
                try
                {
                    using (var fs = new FileStream(_songsPath + fileName, FileMode.Create))
                    {
                        await song.CopyToAsync(fs);
                    }
                    return Ok(fileName);
                }
                catch (Exception exception)
                {
                    return StatusCode(500);
                }
            }
            return BadRequest();

        }

        [HttpPost("upload/image")]
        public async Task<ActionResult> UploadImageFile(IFormFile image)
        {
            if (image != null)
            {
                string fileName = Guid.NewGuid().ToString() + image.FileName;
                try
                {
                    using (var fs = new FileStream(_imagesPath + fileName, FileMode.Create))
                    {
                        await image.CopyToAsync(fs);
                    }
                    return Ok(fileName);
                }
                catch (Exception exception)
                {
                    return StatusCode(500);
                }
            }
            return BadRequest();
        }

        [HttpGet("get/song/{path}")]
        public async Task<IActionResult> GetSongFile(string path)
        {
            byte[] content = await System.IO.File.ReadAllBytesAsync(_songsPath + path);

            return File(content, "audio/*", path);
        }

        [HttpGet("get/image/{path}")]
        public async Task<IActionResult> GetImageFile(string path)
        {
            byte[] content = await System.IO.File.ReadAllBytesAsync(_imagesPath + path);

            return File(content, "image/*", path);
        }

    }
}
