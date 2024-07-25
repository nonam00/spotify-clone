using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Application.Files.Commands.UploadFile;
using Application.Files.Commands.DeleteFile;

namespace WebAPI.Controllers
{
    [ApiVersionNeutral]
    [Route("{version:apiVersion}/files")]
    public class FilesController : BaseController
    {
        private readonly string _fullBucketUrl;
        private readonly string _songsPath;

        public FilesController(IConfiguration configuration)
        {
            _fullBucketUrl = configuration["AwsOptions:ServiceURL"]!
                .Insert(8, (configuration["AwsOptions:BucketName"]! + "."));

            _songsPath = Directory.GetCurrentDirectory() + "/Files/Songs/";

            if (!Directory.Exists(_songsPath))
            {
                Directory.CreateDirectory(_songsPath);
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
        [HttpPost("song")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UploadSongFile(IFormFile song)
        {
            if (song is null)
            {
                return BadRequest();
            }

            try
            {
                string fileName = Guid.NewGuid().ToString() + song.FileName;
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
        [HttpPost("image")]
        [Produces("application/json")]
        public async Task<ActionResult> UploadImageFile(IFormFile image)
        {
            var command = new UploadFileCommand
            {
                FileStream = image.OpenReadStream(),
                ContentType = image.ContentType.Split("/")[0]
            };
            var path = await Mediator.Send(command);
            return Ok(new { path = path });
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
        [HttpGet("song/{path}")]
        public IActionResult GetSongFile(string path)
        {
            string fullPath = _songsPath + path;
            if (!System.IO.File.Exists(fullPath))
            {
                return BadRequest();
            }
            return PhysicalFile(fullPath, "application/ostet-stream", enableRangeProcessing: true);
        }

        /// <summary>
        /// Deletes a file from AWS S3
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     DELETE /{filename}
        /// 
        /// </remarks>
        /// <param name="filename">Filename</param>
        /// <response code="200">Success</response>
        [Authorize]
        [HttpDelete("image/{filename}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteFile(string filename)
        {
            string file = $"image/{filename}";
            var command = new DeleteFileCommand
            {
                FileName = file
            };
            await Mediator.Send(command);
            return NoContent();
        }
    }
}
