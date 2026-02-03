using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Domain.Models;
using Application.Users.Models;
using Application.Users.Queries.GetUserInfo;
using Application.Users.Commands.UpdatePassword;
using Application.Users.Commands.UpdateUser;
using Application.Songs.Models;
using Application.Songs.Queries.GetLikedSongList;
using Application.Users.Queries.CheckLike;
using Application.Users.Commands.LikeSong;
using Application.Users.Commands.UnlikeSong;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Route("{version:apiVersion}/users"), ApiVersionNeutral]
public class UsersController : BaseController
{
    private Guid UserId => GetGuidClaim("userId");

    /// <summary>
    /// Gets user info
    /// </summary>
    /// <returns>Returns new user ID</returns>
    /// <response code="200">Success</response>
    /// <response code="401">If user is unauthorized (doesn't have jwt token)</response>
    [HttpGet("info"), Authorize(Policy = AuthorizationPolicies.UserOnly)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserInfo>> GetUserInfo(CancellationToken cancellationToken)
    {
        var query = new GetUserInfoQuery(UserId);
        var result = await Mediator.Send(query, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        if (result.IsFailure && result.Error == UserDomainErrors.NotActive)
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }

    /// <summary>
    /// Updates user info
    /// </summary>
    /// <param name="updateUserInfoDto">Form with user info</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="204">Success</response>
    /// <response code="401">If user is unauthorized</response>
    [HttpPut, Authorize(Policy = AuthorizationPolicies.UserOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateUserInfo(
        UpdateUserInfoDto updateUserInfoDto, CancellationToken cancellationToken)
    {
        var updateUserCommand = new UpdateUserCommand(
            UserId: UserId, FullName: updateUserInfoDto.FullName, AvatarPath: updateUserInfoDto.AvatarId);
        var result = await Mediator.Send(updateUserCommand, cancellationToken);
        
        if (result.IsSuccess)
        {
            return NoContent();
        }
        return BadRequest(new { Detail = result.Error.Description });
    }

    /// <summary>
    /// Changes user password
    /// </summary>
    /// <param name="updateUserPasswordDto">Current and new passwords</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="204">Success</response>
    /// <response code="401">If user is unauthorized</response>
    [HttpPut("password"), Authorize(Policy = AuthorizationPolicies.UserOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateUserPassword(
        UpdateUserPasswordDto updateUserPasswordDto, CancellationToken cancellationToken)
    {
        var updatePasswordCommand = new UpdatePasswordCommand(
            UserId: UserId,
            CurrentPassword: updateUserPasswordDto.CurrentPassword,
            NewPassword: updateUserPasswordDto.NewPassword);
        var result = await Mediator.Send(updatePasswordCommand, cancellationToken);
        
        if (result.IsSuccess)
        {
            return NoContent();
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }

    /// <summary>
    /// Gets all liked song for certain user
    /// </summary>
    /// <returns>Returns LikedSongListVm</returns>
    /// <response code="200">Success</response>
    /// <response code="401">If user is unauthorized</response>
    [HttpGet("songs"), Authorize(Policy = AuthorizationPolicies.UserOnly)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SongListVm>> GetLikedSongList(CancellationToken cancellationToken)
    {
        var query = new GetLikedSongListQuery(UserId);
        var result = await Mediator.Send(query, cancellationToken);

        if (result.IsSuccess)
        {
            return result.Value;
        }
        
        throw new Exception(result.Error.Description);
    }

    /// <summary>
    /// Checks for like
    /// </summary>
    /// <param name="songId">ID of liked song</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns LikedSongVm</returns>
    /// <response code="200">Success</response>
    /// <response code="401">If user is unauthorized</response>
    [HttpGet("songs/{songId:guid}"), Authorize(Policy = AuthorizationPolicies.UserOnly)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetLikedSong(Guid songId, CancellationToken cancellationToken)
    {
        var query = new CheckLikeQuery(UserId: UserId, SongId: songId);
        var result = await Mediator.Send(query, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { check = result.Value});
        }
        
        throw new Exception(result.Error.Description);
    }

    /// <summary>
    /// Add song to user favorites collection 
    /// </summary>
    /// <param name="songId">ID of song to like</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns string</returns>
    /// <response code="201">Created</response>
    /// <response code="401">If user is unauthorized</response>
    [HttpPost("songs/{songId:guid}"), Authorize(Policy = AuthorizationPolicies.UserOnly)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LikeSong(Guid songId, CancellationToken cancellationToken)
    {
        var command = new LikeSongCommand(UserId: UserId, SongId: songId);
        var result = await Mediator.Send(command, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Created();
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }

    /// <summary>
    /// Delete song from user favorites collection
    /// </summary>
    /// <param name="songId">ID of the song to remove from user favorites</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="204">Success</response>
    /// <response code="401">If user is unauthorized</response>
    [HttpDelete("songs/{songId:guid}"), Authorize(Policy = AuthorizationPolicies.UserOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UnlikeSong(Guid songId, CancellationToken cancellationToken)
    {
        var command = new UnlikeSongCommand(UserId: UserId, SongId: songId);
        var result = await Mediator.Send(command, cancellationToken);
       
        if (result.IsSuccess)
        {
            return NoContent();
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }
}