using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

using Application.Files.Enums;
using Application.Files.Queries.GetFile;

namespace WebAPI.Controllers;

[Route("{version:apiVersion}/files"), ApiVersionNeutral]
public class FilesController : BaseController
{
    /// <summary>
    /// Serves file streams from S3 to Frontend
    /// </summary>
    /// <param name="type">Media type (also decides the bucket)</param>
    /// <param name="name">Name of the file to get</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns file stream</returns>
    /// <response code="200">Success</response>
    [HttpGet("{type}/{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFile(string type, string name, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse(type, true, out MediaType mediaType))
        {
            return BadRequest();
        }
        
        var query = new GetFileQuery
        {
            Name = name,
            MediaType = mediaType
        };
        
        var stream = await Mediator.Send(query, cancellationToken);
        return File(stream, "application/octet-stream", true);
    }
}