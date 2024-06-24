using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Uploads the song file
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     POST /song
        /// 
        /// </remarks>
        /// <response code="201">Success</response>
        /// <response code="401">If user is unauthorized</response>
        [Authorize]
        //[ValidateAntiForgeryToken]
        [HttpPost("song")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
                    return Ok(new { path = fileName });
                }
                catch
                {
                    return StatusCode(500);
                }
            }
            return BadRequest();

        }

        /// <summary>
        /// Uploads the image file
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     POST /image
        /// 
        /// </remarks>
        /// <response code="201">Success</response>
        /// <response code="401">If user is unauthorized</response>
        [Authorize]
        //[ValidateAntiForgeryToken]
        [HttpPost("image")]
        [Produces("application/json")]
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
                    return Ok(new { path = fileName });
                }
                catch
                {
                    return StatusCode(500);
                }
            }
            return BadRequest();
        }

        /// <summary>
        /// Gets the song file by path
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     GET /song/{path}
        /// 
        /// </remarks>
        /// <param name="path">Filename</param>
        /// <response code="200">Success</response>
        /// <response code="401">If user is unauthorized</response>
        [Authorize]
        //[ValidateAntiForgeryToken]
        [HttpGet("song/{path}")]
        public async Task<IActionResult> GetSongFile(string path)
        {
            if(!System.IO.File.Exists(_songsPath + path))
            {
              return BadRequest();
            }
            byte[] content = await System.IO.File.ReadAllBytesAsync(_songsPath + path);

            return File(content, "audio/*", path);
        }

        /// <summary>
        /// Gets the image file by path
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     GET /image/{path}
        /// 
        /// </remarks>
        /// <param name="path">Filename</param>
        /// <response code="200">Success</response>
        [HttpGet("image/{path}")]
        public async Task<IActionResult> GetImageFile(string path)
        {
            if(!System.IO.File.Exists(_imagesPath + path))
            {
              return BadRequest();
            }
            byte[] content = await System.IO.File.ReadAllBytesAsync(_imagesPath + path);

            return File(content, "image/*", path);
        }
    }
}
