using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Application.Songs.Commands.CreateSong;
using Application.Songs.Models;
using Application.Songs.Queries.GetAllSongs;
using Application.Songs.Queries.GetNewestSongList;
using Application.Songs.Queries.GetSongById;
using Application.Songs.Queries.GetSongListBySearch;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Produces("application/json")]
[Route("{version:apiVersion}/songs"), ApiVersionNeutral]
public class SongsController : BaseController
{
    /// <summary>
    /// Gets songs
    /// </summary>
    /// <returns>Returns SongListVm</returns>
    /// <response code="200">Success</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<SongListVm>> GetAllSongs(CancellationToken cancellationToken)
    {
        var query = new GetAllSongsQuery();
        var result = await Mediator.Send(query, cancellationToken);
        
        if (result.IsSuccess)
        {
            return result.Value;
        }

        throw new Exception(result.Error.Description);
    }

    /// <summary>
    /// Gets newest songs
    /// </summary>
    /// <returns>Returns SongListVm</returns>
    /// <response code="200">Success</response>
    [HttpGet("newest")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<SongListVm>> GetNewestSongs(CancellationToken cancellationToken)
    {
        var query = new GetNewestSongListQuery();
        var result = await Mediator.Send(query, cancellationToken);
        
        if (result.IsSuccess)
        {
            return result.Value;
        }
        
        throw new Exception(result.Error.Description);
    }

    /// <summary>
    /// Gets certain song by ID
    /// </summary>
    /// <param name="songId">Song ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns SongVm</returns>
    /// <response code="200">Success</response>
    [HttpGet("{songId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<SongVm>> GetSongById(Guid songId, CancellationToken cancellationToken)
    {
        var query = new GetSongByIdQuery(songId);
        var result = await Mediator.Send(query, cancellationToken);
        
        if (result.IsSuccess)
        {
            return result.Value;
        }
        
        return BadRequest(result.Error.Description);
    }

    /// <summary>
    /// Gets songs satisfying the search request
    /// </summary>
    /// <param name="searchSongDto">search query and search criteria</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns SongListVm</returns>
    /// <response code="200">Success</response>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<SongListVm>> GetSongListBySearch(
        [FromQuery] SearchSongDto searchSongDto, CancellationToken cancellationToken)
    {
        var query = new GetSongListBySearchQuery(
            SearchString: searchSongDto.SearchString, SearchCriteria: searchSongDto.SearchCriteria);
        var result = await Mediator.Send(query, cancellationToken);

        if (result.IsSuccess)
        {
            return result.Value;
        }
        
        throw new Exception(result.Error.Description);
    }

    /// <summary>
    /// Creates new song 
    /// </summary>
    /// <param name="createSongDto">createSongDto object</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns created song ID</returns>
    /// <response code="201">Success</response>
    /// <response code="401">If the user is unauthorized</response>
    [HttpPost, Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Guid>> UploadSong(CreateSongDto createSongDto, CancellationToken cancellationToken)
    {
        var command = new CreateSongCommand(
            UserId: UserId,
            Title: createSongDto.Title,
            Author: createSongDto.Author,
            SongPath: createSongDto.AudioId.ToString(),
            ImagePath: createSongDto.ImageId.ToString()
        );
        var result = await Mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return result.Value;
        }
        
        throw new Exception(result.Error.Description);
    }
}