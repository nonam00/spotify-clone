﻿using AutoMapper;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

using Application.Users.Commands.CreateUser;
using Application.Users.Queries.Login;
using Application.Users.Queries.GetUserInfo;

using WebAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers
{
    [ApiVersionNeutral]
    [Produces("application/json")]
    [Route("{version:apiVersion}/users")]
    public class UsersController(IMapper mapper) : BaseController
    {
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Registries the new user
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /register
        ///     {
        ///         email: "user@example.com",
        ///         password "password1234"
        ///     }
        ///     
        /// </remarks>
        /// <param name="registerDto">RegisterDto object</param>
        /// <returns>Returns access token in cookies</returns>
        /// <response code="201">Success</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var command = _mapper.Map<CreateUserCommand>(registerDto);
            var accessToken = await Mediator.Send(command);

            HttpContext.Response.Cookies.Append("token", accessToken);

            return Ok();
        }

        /// <summary>
        /// Request to get user JWT token
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /login
        ///     {
        ///         email: "user@example.com",
        ///         password "password1234"
        ///     }
        /// 
        /// </remarks>
        /// <param name="loginDto">LoginDto object</param>
        /// <returns>Returns access token in cookies</returns>
        /// <response code="200">Success</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var query = _mapper.Map<LoginQuery>(loginDto);
            var accessToken = await Mediator.Send(query);

            HttpContext.Response.Cookies.Append("token", accessToken);

            return Ok();
        }

        /// <summary>
        /// Clears the user's cookies, resulting in a logout
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /logout
        /// 
        /// </remarks>
        /// <response code="205">Success</response>
        /// <response code="401">If user is unauthorized (doesn't have jwt token)</response>
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status205ResetContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            return StatusCode(205);
        }

        /// <summary>
        /// Gets info about user using jwt token
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /info
        /// 
        /// </remarks>
        /// <returns>Returns new user ID</returns>
        /// <response code="200">Success</response>
        /// <response code="401">If user is unauthorized (doesn't have jwt token)</response>
        [Authorize]
        //[ValidateAntiForgeryToken]
        [HttpGet("info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserInfo>> GetUserInfo()
        {
            var query = new GetUserInfoQuery()
            {
                UserId = UserId
            };
            var info = await Mediator.Send(query);
            return Ok(info);
        }
    }
}
