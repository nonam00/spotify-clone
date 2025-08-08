using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Application.Files.Commands.UploadFile;
using Application.Files.Commands.DeleteFile;

namespace WebAPI.Controllers;

[Route("{version:apiVersion}/files"), ApiVersionNeutral]
public class FilesController : BaseController
{
    private readonly string _fullBucketUrl;
    private readonly string _songsPath;

    public FilesController(IConfiguration configuration)
    {
        _fullBucketUrl = configuration["AwsOptions:ServiceURL"]!
            .Insert(8, configuration["AwsOptions:BucketName"]! + ".");

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
    [HttpPost("song"), Authorize]
    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> UploadSongFile(IFormFile song, CancellationToken cancellationToken)
    {
        var fileName = Guid.NewGuid() + song.FileName;
        try
        {
            await using (var fs = new FileStream(_songsPath + fileName, FileMode.Create))
            {
                await song.CopyToAsync(fs, cancellationToken);
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
    [HttpPost("image"), Authorize]
    [Produces("application/json")]
    public async Task<ActionResult> UploadImageFile(IFormFile image, CancellationToken cancellationToken)
    {
        var command = new UploadFileCommand
        {
            FileStream = image.OpenReadStream(),
            ContentType = image.ContentType.Split("/")[0]
        };
        //var path = await Mediator.Send(command);
        //return Ok(new { path = path });
        return Ok(new { path = "empty" });
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
    public Task<IActionResult> GetSongFile(string path)
    {
        var fullPath = _songsPath + path;
        return !System.IO.File.Exists(fullPath)
            ? Task.FromResult<IActionResult>(BadRequest())
            : Task.FromResult<IActionResult>(PhysicalFile(fullPath, "application/octet-stream", true));
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
    public Task<IActionResult> GetImageFile(string filename)
    {
        //return Task.FromResult<IActionResult>(Redirect(_fullBucketUrl + "image/" + filename));
        return Task.FromResult<IActionResult>(Ok());
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
    [HttpDelete("image/{filename}"), Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteFile(string filename)
    {
        var command = new DeleteFileCommand
        {
            FileName = $"image/{filename}"
        };
        //await Mediator.Send(command);
        return NoContent();
    }
}