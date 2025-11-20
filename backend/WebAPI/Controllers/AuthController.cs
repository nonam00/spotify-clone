using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

using Application.Users.Commands.ActivateUser;
using Application.Users.Commands.CreateUser;
using Application.Users.Commands.DeleteRefreshToken;
using Application.Users.Queries.LoginByCredentials;
using Application.Users.Queries.LoginByRefreshToken;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Route("{version:apiVersion}/auth"), ApiVersionNeutral]
public class AuthController : BaseController
{
    /// <summary>
    /// Registries the new user
    /// </summary>
    /// <param name="userCredentialsDto">UserCredentials object</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns access token in cookies</returns>
    /// <response code="201">Created</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> RegisterUser(
        [FromForm] UserCredentialsDto userCredentialsDto, CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(Email: userCredentialsDto.Email, Password: userCredentialsDto.Password);
        var result = await Mediator.Send(command, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Created();
        }

        return BadRequest(new { Detail = result.Error.Description });
    }

    /// <summary>
    /// Activates user account using confirmation code from email
    /// </summary>
    /// <param name="activateUserDto">User email and confirmation code from email</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns access token in cookies</returns>
    /// <response code="302">Found</response>
    [HttpGet("activate")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    public async Task<IActionResult> ActivateUser(
        [FromQuery] ActivateUserDto activateUserDto, CancellationToken cancellationToken)
    {
        var command = new ActivateUserCommand(Email: activateUserDto.Email, ConfirmationCode: activateUserDto.Code);
        var result = await Mediator.Send(command, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Redirect("http://localhost:3000");
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }

    /// <summary>
    /// Request to get token pair
    /// </summary>
    /// <param name="userCredentialsDto">LoginDto object</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns access token in cookies</returns>
    /// <response code="200">Success</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UserLogin(
        [FromForm] UserCredentialsDto userCredentialsDto, CancellationToken cancellationToken)
    {
        var query = new LoginByCredentialsQuery(Email: userCredentialsDto.Email, Password: userCredentialsDto.Password);
        var result = await Mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { Detail = result.Error.Description });
        }
        
        HttpContext.Response.Cookies.Append("access_token", result.Value.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            MaxAge = TimeSpan.FromHours(12)
        });
        
        HttpContext.Response.Cookies.Append("refresh_token", result.Value.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            MaxAge = TimeSpan.FromDays(7)
        });
        
        return Ok();
    }

    /// <summary>
    /// Gets access token using refresh token
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns access token in cookies</returns>
    /// <response code="200">Success</response>
    [Route("refresh"), AcceptVerbs("GET", "POST")]
    public async Task<IActionResult> UserRefresh(CancellationToken cancellationToken)
    {
        if (!HttpContext.Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
        {
            return Unauthorized();
        }

        var loginQuery = new LoginByRefreshTokenQuery(refreshToken);
        var result = await Mediator.Send(loginQuery, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { Detail = result.Error.Description });
        }
        
        HttpContext.Response.Cookies.Append("access_token", result.Value.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            MaxAge = TimeSpan.FromHours(12)
        });
        
        HttpContext.Response.Cookies.Append("refresh_token", result.Value.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            MaxAge = TimeSpan.FromDays(7)
        });

        return Ok();
    }

    /// <summary>
    /// Clears the user's cookies, resulting in a logout, and deleting current refresh token from db
    /// </summary>
    /// <response code="205">Reset Content</response>
    /// <response code="401">If user is unauthorized (doesn't have jwt token)</response>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status205ResetContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UserLogout(CancellationToken cancellationToken)
    {
        Response.Cookies.Delete("access_token");
        
        if (HttpContext.Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
        {
            Response.Cookies.Delete("refresh_token");
            var deleteTokenCommand = new DeleteRefreshTokenCommand(refreshToken);
            await Mediator.Send(deleteTokenCommand, cancellationToken);
        }
        
        return StatusCode(205);
    }
    
    [HttpPost("moderators/login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ModeratorLogin(
        [FromForm] UserCredentialsDto userCredentialsDto, CancellationToken cancellationToken)
    {
        var query = new Application.Moderators.Queries.LoginByCredentials.LoginByCredentialsQuery(
            Email: userCredentialsDto.Email,
            Password: userCredentialsDto.Password);
        var result = await Mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { Detail = result.Error.Description });
        }
        
        HttpContext.Response.Cookies.Append("access_token", result.Value, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            MaxAge = TimeSpan.FromHours(24)
        });
        
        return Ok();
    }
    
    [HttpPost("moderators/logout")]
    [ProducesResponseType(StatusCodes.Status205ResetContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public Task<IActionResult> ModeratorLogout()
    {
        Response.Cookies.Delete("access_token");
        return Task.FromResult<IActionResult>(StatusCode(205));
    }
}