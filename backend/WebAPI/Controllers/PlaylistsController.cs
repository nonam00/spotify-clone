using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Application.Playlists.Models;
using Application.Playlists.Queries.GetPlaylistById;
using Application.Playlists.Commands.UpdatePlaylist;
using Application.Playlists.Commands.AddSongToPlaylist;
using Application.Playlists.Commands.AddSongsToPlaylist;
using Application.Playlists.Commands.RemoveSongFromPlaylist;
using Application.Playlists.Errors;
using Application.Playlists.Queries.GetFullPlaylistList;
using Application.Playlists.Queries.GetPlaylistListByCount;
using Application.Songs.Models;
using Application.Songs.Queries.GetLikedSongListForPlaylist;
using Application.Songs.Queries.GetLikedSongListForPlaylistBySearch;
using Application.Songs.Queries.GetSongListByPlaylistId;
using Application.Users.Commands.CreatePlaylist;
using Application.Users.Commands.DeletePlaylist;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Produces("application/json")]
[Route("{version:apiVersion}/playlists"), Authorize, ApiVersionNeutral]
public class PlaylistsController : BaseController
{
    /// <summary>
    /// Gets certain user playlist 
    /// </summary>
    /// <param name="playlistId">ID of playlist</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns PlaylistVm</returns>
    /// <response code="201">Success</response>
    /// <response code="401">If user is unauthorized</response>
    [HttpGet("{playlistId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PlaylistVm>> GetPlaylist(Guid playlistId, CancellationToken cancellationToken)
    {
        var query = new GetPlaylistByIdQuery(PlaylistId: playlistId, UserId: UserId);
        
        var result = await Mediator.Send(query, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        if (result.Error.Code == nameof(PlaylistErrors.NotFound))
        {
            return NotFound(new { Detail = result.Error.Description });
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }

    /// <summary>
    /// Gets list of user playlist 
    /// </summary>
    /// <returns>Returns PlaylistListVm</returns>
    /// <response code="201">Success</response>
    /// <response code="401">If user is unauthorized</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PlaylistListVm>> GetPlaylistList(CancellationToken cancellationToken)
    {
        var query = new GetFullPlaylistListQuery(UserId);
        
        var result = await Mediator.Send(query, cancellationToken);
        
        if (result.IsSuccess)
        {
            return result.Value;
        }

        throw new Exception(result.Error.Description);
    }

    /// <summary>
    /// Gets certain quantity of user playlists
    /// </summary>
    /// <param name="count">Count of playlist</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns PlaylistListVm</returns>
    /// <response code="201">Success</response>
    /// <response code="401">If user is unauthorized</response>
    [HttpGet("certain/{count:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PlaylistListVm>> GetPlaylistListByCount(
        int count, CancellationToken cancellationToken)
    {
        var query = new GetPlaylistListByCountQuery(UserId, count);
        
        var result = await Mediator.Send(query, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        throw new Exception(result.Error.Description);
    }

    /// <summary>
    /// Creates new user playlist 
    /// </summary>
    /// <returns>Returns Guid</returns>
    /// <response code="201">Success</response>
    /// <response code="401">If user is unauthorized</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Guid>> CreatePlaylist(CancellationToken cancellationToken)
    {
        var command = new CreatePlaylistCommand(UserId);
        
        var result = await Mediator.Send(command, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        return BadRequest(new { Detail = result.Error.Description });
    }

    /// <summary>
    /// Updates playlist information
    /// </summary>
    /// <param name="playlistId">ID of the playlist that needs to be updated</param>
    /// <param name="updatePlaylistDto">object with new info</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="204">Success</response>
    /// <response code="401">If the user is unauthorized</response>
    [HttpPut("{playlistId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdatePlaylist(
        Guid playlistId, UpdatePlaylistDto updatePlaylistDto, CancellationToken cancellationToken)
    {
        var updatePlaylistCommand = new UpdatePlaylistCommand(
            UserId: UserId,
            PlaylistId: playlistId,
            Title: updatePlaylistDto.Title,
            Description: updatePlaylistDto.Description,
            ImagePath: updatePlaylistDto.ImageId);
        
        var result = await Mediator.Send(updatePlaylistCommand, cancellationToken);
        
        if (result.IsSuccess)
        {
            return NoContent();
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }
        
    /// <summary>
    /// Deletes user playlist
    /// </summary>
    /// <param name="playlistId">ID of the playlist that needs to be deleted</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="204">Success</response>
    /// <response code="401">If the user is unauthorized</response>
    [HttpDelete("{playlistId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeletePlaylist(Guid playlistId, CancellationToken cancellationToken)
    {
        var deletePlaylistCommand = new DeletePlaylistCommand(UserId: UserId, PlaylistId: playlistId);
        
        var result = await Mediator.Send(deletePlaylistCommand, cancellationToken);
        
        if (result.IsSuccess)
        {
            return NoContent();
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }
        
    /// <summary>
    /// Gets songs from user playlist
    /// </summary>
    /// <param name="playlistId">ID of the playlist from which the songs will be gotten</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns SongListVm</returns>
    /// <response code="204">Success</response>
    /// <response code="401">If the user is unauthorized</response>
    [HttpGet("{playlistId:guid}/songs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SongListVm>> GetSongsInPlaylist(Guid playlistId, CancellationToken cancellationToken)
    {
        var query = new GetSongListByPlaylistIdQuery(playlistId);
        var result = await Mediator.Send(query, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        throw new Exception(result.Error.Description);
    }

    /// <summary>
    /// Adds the song to the playlist
    /// </summary>
    /// <param name="playlistId">ID of the playlist to which the song is adding</param>
    /// <param name="songId">ID of the song which is adding to the playlist</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns db key of created relation</returns>
    /// <response code="201">Success</response>
    /// <response code="401">If the user is unauthorized</response>
    [HttpPost("{playlistId:guid}/songs/{songId:guid}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddSongToPlaylist(
        Guid playlistId, Guid songId, CancellationToken cancellationToken)
    {
        var command = new AddSongToPlaylistCommand(UserId: UserId, PlaylistId: playlistId, SongId: songId);
        var result = await Mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return BadRequest(new { Detail = result.Error.Description });
    }


    /// <summary>
    /// Adds songs to the playlist
    /// </summary>
    /// <param name="playlistId">ID of the playlist to which the song is adding</param>
    /// <param name="songId">ID of the song which is adding to the playlist</param>
    /// <param name="addSongsToPlaylistDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns db key of created relation</returns>
    /// <response code="201">Success</response>
    /// <response code="401">If the user is unauthorized</response>
    [HttpPost("{playlistId:guid}/songs/")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddSongsToPlaylist(
            Guid playlistId, AddSongsToPlaylistDto addSongsToPlaylistDto, CancellationToken cancellationToken)
    {
        var command = new AddSongsToPlaylistCommand(
            UserId: UserId, PlaylistId: playlistId, SongIds: addSongsToPlaylistDto.SongIds);
        var result = await Mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return BadRequest(new { Detail = result.Error.Description });
    }
        
    /// <summary>
    /// Removes the song from the playlist
    /// </summary>
    /// <param name="playlistId">ID of the playlist from which the song is removing</param>
    /// <param name="songId">ID of the song which is removing from the playlist</param>
    /// <response code="204">Success</response>
    /// <response code="401">If the user is unauthorized</response>
    [HttpDelete("{playlistId:guid}/songs/{songId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteSongFromPlaylist(Guid playlistId, Guid songId)
    {
        var command = new RemoveSongFromPlaylistCommand(UserId: UserId, PlaylistId: playlistId, SongId: songId);
        var result = await Mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return NoContent();
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }
        
    /// <summary>
    /// Gets user favorite songs which are not in the playlist
    /// </summary>
    /// <param name="playlistId">ID of the playlist</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns LikedSongListVm</returns>
    /// <response code="200">Success</response>
    /// <response code="401">If the user is unauthorized</response>
    [HttpGet("{playlistId:guid}/liked")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SongListVm>> GetLikedSongsNotInPlaylist(
        Guid playlistId, CancellationToken cancellationToken)
    {
        var query = new GetLikedSongListForPlaylistQuery(UserId: UserId, PlaylistId: playlistId);
        
        var result = await Mediator.Send(query, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        throw new Exception(result.Error.Description);
    }
        
    /// <summary>
    /// Gets user favorite songs which are not in the playlist by search string
    /// </summary>
    /// <param name="playlistId">ID of the playlist</param>
    /// <param name="searchString">User search request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns LikedSongListVm></returns>
    /// <response code="200">Success</response>
    /// <response code="401">If the user is unauthorized</response>
    [HttpGet("{playlistId:guid}/liked/{searchString}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SongListVm>> GetLikedSongsNotInPlaylistBySearchString(
        Guid playlistId, string searchString, CancellationToken cancellationToken)
    {
        var query = new GetLikedSongListForPlaylistBySearchQuery(
            UserId: UserId, PlaylistId: playlistId, SearchString: searchString);
        
        var result = await Mediator.Send(query, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        throw new Exception(result.Error.Description);
    }
}