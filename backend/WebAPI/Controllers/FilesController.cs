using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

using Application.Files.Enums;
using Application.Files.Queries.GetPresignedUrl;

namespace WebAPI.Controllers;

[Route("{version:apiVersion}/files"), ApiVersionNeutral]
public class FilesController : BaseController
{
    /// <summary>
    /// Redirects to s3 presigned urls
    /// </summary>
    /// <param name="type">Media type (also decides the bucket)</param>
    /// <param name="name">Name of the file to get</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns file stream</returns>
    /// <response code="200">Success</response>
    [HttpGet("{type}/{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPresignedUrl(string type, string name, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse(type, true, out MediaType mediaType))
        {
            return BadRequest();
        }

        var query = new GetPresignedUrlQuery
        {
            Name = name,
            MediaType = mediaType
        };

        var url = await Mediator.Send(query, cancellationToken);
        return Redirect(url);
    }
}