using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Application.Users.Models;
using Application.Users.Queries.GetUserInfo;
using Application.Users.Commands.UpdatePassword;
using Application.Users.Commands.UpdateUser;

using Application.LikedSongs.Models;
using Application.LikedSongs.Queries.CheckLikedSong;
using Application.LikedSongs.Commands.CreateLikedSong;
using Application.LikedSongs.Commands.DeleteLikedSong;
using Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongList;

using Application.Files.Commands.DeleteFile;
using Application.Files.Commands.UploadFile;
using Application.Files.Enums;

using WebAPI.Models;

namespace WebAPI.Controllers;

[Route("{version:apiVersion}/users"), ApiVersionNeutral]
public class UsersController : BaseController
{
    /// <summary>
    /// Gets user info
    /// </summary>
    /// <returns>Returns new user ID</returns>
    /// <response code="200">Success</response>
    /// <response code="401">If user is unauthorized (doesn't have jwt token)</response>
    [HttpGet("info"), Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserInfo>> GetUserInfo(CancellationToken cancellationToken)
    {
        var query = new GetUserInfoQuery
        {
            UserId = UserId
        };
        var info = await Mediator.Send(query, cancellationToken);
        return Ok(info);
    }

    /// <summary>
    /// Updates user info
    /// </summary>
    /// <param name="updateUserInfoDto">Form with user info</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="204">Success</response>
    /// <response code="401">If user is unauthorized</response>
    [HttpPut, Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateUserInfo(
        [FromForm] UpdateUserInfoDto updateUserInfoDto, CancellationToken cancellationToken)
    {
        string? newImagePath = null;
        if (updateUserInfoDto.Avatar is not null)
        {
            var uploadImageCommand = new UploadFileCommand
            {
                FileStream = updateUserInfoDto.Avatar.OpenReadStream(),
                MediaType = MediaType.Image
            };
            newImagePath = await Mediator.Send(uploadImageCommand, cancellationToken);
        }

        var updateUserCommand = new UpdateUserCommand
        {
            UserId = UserId,
            FullName = updateUserInfoDto.FullName,
            AvatarPath = newImagePath
        };

        var oldImagePath = await Mediator.Send(updateUserCommand, cancellationToken);
        
        if (oldImagePath is not null)
        {
            var deleteImageCommand = new DeleteFileCommand
            {
                Name = oldImagePath,
                MediaType = MediaType.Image
            };
            await Mediator.Send(deleteImageCommand, cancellationToken);
        }
        
        return NoContent();
    }

    /// <summary>
    /// Changes user password
    /// </summary>
    /// <param name="updateUserPasswordDto">Current and new passwords</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="204">Success</response>
    /// <response code="401">If user is unauthorized</response>
    [HttpPut("password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateUserPassword(
        UpdateUserPasswordDto updateUserPasswordDto, CancellationToken cancellationToken)
    {
        var updatePasswordCommand = new UpdatePasswordCommand
        {
            UserId = UserId,
            CurrentPassword = updateUserPasswordDto.CurrentPassword,
            NewPassword = updateUserPasswordDto.NewPassword
        };
        await Mediator.Send(updatePasswordCommand, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Gets all liked song for certain user
    /// </summary>
    /// <returns>Returns LikedSongListVm</returns>
    /// <response code="200">Success</response>
    /// <response code="401">If user is unauthorized</response>
    [HttpGet("songs"), Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LikedSongListVm>> GetLikedSongList(CancellationToken cancellationToken)
    {
        var query = new GetLikedSongListQuery
        {
            UserId = UserId
        };
        var vm = await Mediator.Send(query, cancellationToken);
        return Ok(vm);
    }

    /// <summary>
    /// Checks for like
    /// </summary>
    /// <param name="songId">ID of liked song</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns LikedSongVm</returns>
    /// <response code="200">Success</response>
    /// <response code="401">If user is unauthorized</response>
    [HttpGet("songs/{songId:guid}"), Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetLikedSong(Guid songId, CancellationToken cancellationToken)
    {
        var query = new CheckLikedSongQuery
        {
            UserId = UserId,
            SongId = songId
        };
        var check = await Mediator.Send(query, cancellationToken);
        return Ok(new { check });
    }

    /// <summary>
    /// Creates liked song data for certain song and user
    /// </summary>
    /// <param name="songId">ID of song ot like</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns string</returns>
    /// <response code="201">Success</response>
    /// <response code="401">If user is unauthorized</response>
    [HttpPost("songs/{songId:guid}"), Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<string>> CreateLiked(Guid songId, CancellationToken cancellationToken)
    {
        var command = new CreateLikedSongCommand
        {
            UserId = UserId,
            SongId = songId
        };
        var likedId = await Mediator.Send(command, cancellationToken);
        return Ok(likedId);
    }

    /// <summary>
    /// Delete liked song data by song ID
    /// </summary>
    /// <param name="songId">ID of the song to remove from user favorites</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="204">Success</response>
    /// <response code="401">If user is unauthorized</response>
    [HttpDelete("songs/{songId:guid}"), Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteLiked(Guid songId, CancellationToken cancellationToken)
    {
        var command = new DeleteLikedSongCommand
        {
            UserId = UserId,
            SongId = songId
        };
        await Mediator.Send(command, cancellationToken);
        return NoContent();
    }
}