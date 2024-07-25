using AutoMapper;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Application.Users.Commands.CreateUser;
using Application.Users.Queries.Login;
using Application.Users.Queries.GetUserInfo;

using Application.LikedSongs.Queries.GetLikedSongList.GetFullLikedSongList;
using Application.LikedSongs.Queries.CheckLikedSong;
using Application.LikedSongs.Queries.GetLikedSongList;
using Application.LikedSongs.Commands.CreateLikedSong;
using Application.LikedSongs.Commands.DeleteLikedSong;

using WebAPI.Models;

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
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var command = _mapper.Map<CreateUserCommand>(registerDto);
            var accessToken = await Mediator.Send(command);

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
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var query = _mapper.Map<LoginQuery>(loginDto);
            var accessToken = await Mediator.Send(query);

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
            await Task.Run(() => Parallel.ForEach(Request.Cookies.Keys, Response.Cookies.Delete));
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

        /// <summary>
        /// Gets all liked song for certain user
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     GET /songs
        /// 
        /// </remarks>
        /// <returns>Returns LikedSongListVm</returns>
        /// <response code="200">Success</response>
        /// <response code="401">If user is unauthorized</response>
        [Authorize]
        [HttpGet("songs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LikedSongListVm>> GetLikedSongList()
        {
            var query = new GetFullLikedSongListQuery
            {
                UserId = UserId
            };
            var vm = await Mediator.Send(query);
            return Ok(vm);
        }

        /// <summary>
        /// Checks for like
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     GET /songs/{songId}
        /// 
        /// </remarks>
        /// <param name="songId">ID of tiked song</param>
        /// <returns>Returns LikedSongVm</returns>
        /// <response code="200">Success</response>
        /// <response code="401">If user is unauthorized</response>
        [Authorize]
        [HttpGet("songs/{songId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetLikedSong(Guid songId)
        {
            var query = new CheckLikedSongQuery()
            {
                UserId = UserId,
                SongId = songId
            };
            var check = await Mediator.Send(query);
            return check? Ok(check) : Ok();
        }

        /// <summary>
        /// Creates liked song data for certain song and user
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     POST /songs/{songId}
        /// 
        /// </remarks>
        /// <param name="songId">ID of song ot like</param>
        /// <returns>Returns string</returns>
        /// <response code="201">Success</response>
        /// <response code="401">If user is unauthorized</response>
        [Authorize]
        [HttpPost("songs/{songId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<string>> CreateLiked(Guid songId)
        {
            var command = new CreateLikedSongCommand
            {
                UserId = UserId,
                SongId = songId
            };
            var likedId = await Mediator.Send(command);
            return Ok(likedId);
        }

        /// <summary>
        /// Delete liked song data by song ID
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     DELETE /songs/{songId}
        /// 
        /// </remarks>
        /// <param name="songId">
        /// Id of the song to remove from user favorites 
        /// </param>
        /// <response code="204">Success</response>
        /// <response code="401">If user is unauthorized</response>
        [Authorize]
        [HttpDelete("songs/{songId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteLiked(Guid songId)
        {
            var command = new DeleteLikedSongCommand
            {
                UserId = UserId,
                SongId = songId
            };
            await Mediator.Send(command);
            return NoContent();
        }
    }
}
