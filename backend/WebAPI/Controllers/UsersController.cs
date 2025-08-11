using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Application.Users.Models;
using Application.Users.Commands.CreateUser;
using Application.Users.Queries.Login;
using Application.Users.Queries.GetUserInfo;
using Application.LikedSongs.Models;
using Application.LikedSongs.Queries.CheckLikedSong;
using Application.LikedSongs.Commands.CreateLikedSong;
using Application.LikedSongs.Commands.DeleteLikedSong;
using Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongList;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Produces("application/json")]
[Route("{version:apiVersion}/users"), ApiVersionNeutral]
public class UsersController : BaseController
{
    /// <summary>
    /// Registries the new user
    /// </summary>
    /// <param name="userCredentialsDto">UserCredentials object</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns access token in cookies</returns>
    /// <response code="201">Success</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Register(
        [FromForm] UserCredentialsDto userCredentialsDto, CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand
        {
            Email = userCredentialsDto.Email,
            Password = userCredentialsDto.Password
        };
        
        var accessToken = await Mediator.Send(command, cancellationToken);

        HttpContext.Response.Cookies.Append("token", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            MaxAge = TimeSpan.FromHours(12)
        });

        return Ok();
    }

    /// <summary>
    /// Request to get user JWT token
    /// </summary>
    /// <param name="userCredentialsDto">LoginDto object</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns access token in cookies</returns>
    /// <response code="200">Success</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(
        [FromForm] UserCredentialsDto userCredentialsDto, CancellationToken cancellationToken)
    {
        var query = new LoginQuery
        {
            Email = userCredentialsDto.Email,
            Password = userCredentialsDto.Password
        };
        
        var accessToken = await Mediator.Send(query, cancellationToken);

        HttpContext.Response.Cookies.Append("token", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            MaxAge = TimeSpan.FromHours(12)
        });

        return Ok();
    }

    /// <summary>
    /// Clears the user's cookies, resulting in a logout
    /// </summary>
    /// <response code="205">Success</response>
    /// <response code="401">If user is unauthorized (doesn't have jwt token)</response>
    [HttpPost("logout"), Authorize]
    [ProducesResponseType(StatusCodes.Status205ResetContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        await Task.Run(() => Parallel.ForEach(Request.Cookies.Keys, Response.Cookies.Delete));
        return StatusCode(205);
    }

    /// <summary>
    /// Gets info about user using jwt token
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
        return check? Ok(check) : Ok();
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