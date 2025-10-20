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
        
        await Mediator.Send(command, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Activates user account using confirmation code from email
    /// </summary>
    /// <param name="activateUserDto">User email and confirmation code from email</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns access token in cookies</returns>
    /// <response code="200">Success</response>
    [HttpGet("activate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Activate(
        [FromQuery] ActivateUserDto activateUserDto, CancellationToken cancellationToken)
    {
        var command = new ActivateUserCommand
        {
            Email = activateUserDto.Email,
            ConfirmationCode = activateUserDto.Code
        };
        await Mediator.Send(command, cancellationToken);
        return Redirect("http://localhost:3000");
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
    public async Task<IActionResult> Login(
        [FromForm] UserCredentialsDto userCredentialsDto, CancellationToken cancellationToken)
    {
        var query = new LoginByCredentialsQuery
        {
            Email = userCredentialsDto.Email,
            Password = userCredentialsDto.Password
        };
        
        var pair = await Mediator.Send(query, cancellationToken);
        
        HttpContext.Response.Cookies.Append("access_token", pair.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            MaxAge = TimeSpan.FromHours(12)
        });
        
        HttpContext.Response.Cookies.Append("refresh_token", pair.RefreshToken, new CookieOptions
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
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        if (!HttpContext.Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
        {
            return Unauthorized();
        }

        var loginQuery = new LoginByRefreshTokenQuery
        {
            RefreshToken = refreshToken
        };

        var tokenPair = await Mediator.Send(loginQuery, cancellationToken);
        
        HttpContext.Response.Cookies.Append("access_token", tokenPair.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            MaxAge = TimeSpan.FromHours(12)
        });
        
        HttpContext.Response.Cookies.Append("refresh_token", tokenPair.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            MaxAge = TimeSpan.FromDays(7)
        });

        return Ok(new { accessToken = tokenPair.AccessToken });
    }

    /// <summary>
    /// Clears the user's cookies, resulting in a logout, and deleting current refresh token from db
    /// </summary>
    /// <response code="205">Success</response>
    /// <response code="401">If user is unauthorized (doesn't have jwt token)</response>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status205ResetContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        Response.Cookies.Delete("access_token");
        if (HttpContext.Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
        {
            Response.Cookies.Delete("refresh_token");
            var deleteTokenCommand = new DeleteRefreshTokenCommand
            {
                RefreshToken = refreshToken
            };
            await Mediator.Send(deleteTokenCommand, cancellationToken);
        }
        return StatusCode(205);
    }
}