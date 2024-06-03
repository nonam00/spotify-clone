using AutoMapper;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

using Application.Users.Commands.CreateUser;
using Application.Users.Queries;
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
        /// <returns>Returns new user ID</returns>
        /// <response code="200">Success</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Guid>> Register([FromBody] RegisterDto registerDto)
        {
            var command = _mapper.Map<CreateUserCommand>(registerDto);
            var userId = await Mediator.Send(command);
            return Ok(userId);
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
        /// <returns>Returns access token and user data</returns>
        /// <response code="200">Success</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserVm>> Login([FromBody] LoginDto loginDto)
        {
            var query = _mapper.Map<LoginQuery>(loginDto);
            var userVm = await Mediator.Send(query);

            HttpContext.Response.Cookies.Append("token", userVm.AccessToken);

            return Ok(userVm);
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
