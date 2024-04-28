using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using Application.Users.Commands.CreateUser;
using Application.Users.Queries.Login;

using WebAPI.Models;
using Asp.Versioning;

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

        [HttpGet("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> Login([FromQuery] LoginDto loginDto)
        {
            var query = _mapper.Map<LoginQuery>(loginDto);
            var token = await Mediator.Send(query);

            HttpContext.Response.Cookies.Append("cookies", token);

            return Ok(token);
        }
    }
}
