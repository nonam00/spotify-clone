using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Application.Files.Commands.UploadFile;
using Application.Files.Commands.DeleteFile;

namespace WebAPI.Controllers
{
    [ApiVersionNeutral]
    [Route("{version:apiVersion}/files")]
    public class FilesController(IConfiguration configuration) : BaseController
    {
        private readonly string _fullBucketUrl =
            configuration["AwsOptions:ServiceURL"]!
                .Insert(8, (configuration["AwsOptions:BucketName"]! + "."));

        /// <summary>
        /// Uploads the file
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     POST /
        /// 
        /// </remarks>
        /// <response code="201">Success</response>
        /// <response code="401">If user is unauthorized</response>
        [Authorize]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UploadFile(IFormFile file)
        {
            var command = new UploadFileCommand
            {
                FileStream = file.OpenReadStream(),
                ContentType = file.ContentType.Split("/")[0]
            };
            var path = await Mediator.Send(command);
            return Ok(new { path = path });
        }

        /// <summary>
        /// Proxies access to songs in AWS S3
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     GET /song/{filename}
        /// 
        /// </remarks>
        /// <param name="filename">Filename</param>
        /// <response code="200">Success</response>
        /// <response code="401">If user is unauthorized</response>
        [Authorize]
        //[ValidateAntiForgeryToken]
        [HttpGet("song/{filename}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSongFile(string filename)
        {
            return Ok(_fullBucketUrl + "audio/" + filename);
        }

        /// <summary>
        /// Proxies access to images in AWS S3
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     GET /image/{filename}
        /// 
        /// </remarks>
        /// <param name="filename">Filename</param>
        /// <response code="200">Success</response>
        [HttpGet("image/{filename}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetImageFile(string filename)
        {
            return Redirect(_fullBucketUrl + "image/" + filename);
        }

        /// <summary>
        /// Deletes file from AWS S3
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     DELETE /{type}/{filename}
        /// 
        /// </remarks>
        /// <param name="type">File Content Type</param>
        /// <param name="filename">Filename</param>
        /// <response code="200">Success</response>
        [Authorize]
        [HttpDelete("{type}/{filename}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteFile(string type, string filename)
        {
            string file = $"{type}/{filename}";
            Console.WriteLine(file);
            var command = new DeleteFileCommand
            {
                FileName = file
            };
            await Mediator.Send(command);
            return NoContent();
        }
    }
}
