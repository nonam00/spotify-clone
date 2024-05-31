using AutoMapper;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

using Application.Users.Commands.CreateUser;
using Application.Users.Queries;
using Application.Users.Queries.Login;
using Application.Users.Queries.GetUserInfo;

using WebAPI.Models;

namespace WebAPI.Controllers
{
    [ApiVersionNeutral]
    [Produces("application/json")]
    [Route("{version:apiVersion}/users")]
    public class UsersController(IMapper mapper) : BaseController
    {
        private readonly IMapper _mapper = mapper;

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Guid>> Register([FromBody] RegisterDto registerDto)
        {
            var command = _mapper.Map<CreateUserCommand>(registerDto);
            var userId = await Mediator.Send(command);
            return Ok(userId);
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserVm>> Login([FromBody] LoginDto loginDto)
        {
            var query = _mapper.Map<LoginQuery>(loginDto);
            var userVm = await Mediator.Send(query);

            HttpContext.Response.Cookies.Append("token", userVm.AccessToken);

            return Ok(userVm);
        }

        [HttpGet("info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
