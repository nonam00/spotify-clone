using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Application.Users.Commands.ActivateUserByConfirmationCode;
using Application.Users.Commands.CreateUser;
using Application.Users.Commands.DeleteRefreshToken;
using Application.Users.Commands.RestoreUserAccess;
using Application.Users.Commands.SendRestoreToken;
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
    /// <param name="loginCredentialsDto">UserCredentials object</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns access token in cookies</returns>
    /// <response code="201">Created</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> RegisterUser(
        [FromForm] UserRegistrationDto loginCredentialsDto, CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(
            Email: loginCredentialsDto.Email,
            Password: loginCredentialsDto.Password,
            FullName: loginCredentialsDto.FullName);
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
    /// <param name="email">Account email</param>
    /// <param name="code">Confirmation code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns access token in cookies</returns>
    /// <response code="302">Found</response>
    [HttpGet("activate")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    public async Task<IActionResult> ActivateUser(
        [FromQuery] string email, [FromQuery] string code, CancellationToken cancellationToken)
    {
        var command = new ActivateUserByConfirmationCodeCommand(email, code);
        var result = await Mediator.Send(command, cancellationToken);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { Detail = result.Error.Description });
        }
                
        SetAuthCookies(result.Value.AccessToken, result.Value.RefreshToken);
        
        return Redirect("http://localhost:3000");
    }

    /// <summary>
    /// Request to get token pair
    /// </summary>
    /// <param name="loginCredentialsDto">LoginDto object</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns access token in cookies</returns>
    /// <response code="200">Success</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UserLogin(
        [FromForm] LoginCredentialsDto loginCredentialsDto, CancellationToken cancellationToken)
    {
        var query = new LoginByCredentialsQuery(Email: loginCredentialsDto.Email, Password: loginCredentialsDto.Password);
        var result = await Mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { Detail = result.Error.Description });
        }
        
        SetAuthCookies(result.Value.AccessToken, result.Value.RefreshToken);
        
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
        
        SetAuthCookies(result.Value.AccessToken, result.Value.RefreshToken);

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
    
    [HttpPost("send-restore-code")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> SendRestoreCode([FromQuery] string email, CancellationToken cancellationToken)
    {
        var command = new SendRestoreTokenCommand(email);
        var result = await Mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { Detail = result.Error.Description });
        }

        return Created();
    }
    
    [HttpGet("restore-access")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> RestoreAccess(
        [FromQuery] string email, [FromQuery] string code, CancellationToken cancellationToken)
    {
        var command = new RestoreUserAccessCommand(email, code);
        var result = await Mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { Detail = result.Error.Description });
        }
        
        SetAuthCookies(result.Value.AccessToken, result.Value.RefreshToken);

        return Redirect("http://localhost:3000");
    }
    
    [HttpPost("moderators/login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ModeratorLogin(
        [FromForm] LoginCredentialsDto loginCredentialsDto, CancellationToken cancellationToken)
    {
        var query = new Application.Moderators.Queries.LoginByCredentials.LoginByCredentialsQuery(
            Email: loginCredentialsDto.Email,
            Password: loginCredentialsDto.Password);
        var result = await Mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { Detail = result.Error.Description });
        }
        
        SetAuthCookies(result.Value);
        
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

    private void SetAuthCookies(string accessToken, string? refreshToken = null)
    {
        HttpContext.Response.Cookies.Append("access_token", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            MaxAge = TimeSpan.FromHours(12)
        });

        if (refreshToken != null)
        {
            HttpContext.Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                MaxAge = TimeSpan.FromDays(7)
            });
        }
    }
}