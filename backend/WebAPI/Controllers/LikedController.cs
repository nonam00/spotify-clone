using AutoMapper;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Application.LikedSongs.Queries.GetLikedSongList.GetFullLikedSongList;
using Application.LikedSongs.Queries.CheckLikedSong;
using Application.LikedSongs.Queries.GetLikedSongList;
using Application.LikedSongs.Commands.CreateLikedSong;
using Application.LikedSongs.Commands.DeleteLikedSong;


namespace WebAPI.Controllers
{
    [Authorize]
    //[AutoValidateAntiforgeryToken]
    [ApiVersionNeutral]
    [Produces("application/json")]
    [Route("{version:apiVersion}/liked")]
    public class LikedController(IMapper mapper) : BaseController
    {
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Gets all liked song for certain user
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     GET /
        /// 
        /// </remarks>
        /// <returns>Returns LikedSongListVm</returns>
        /// <response code="200">Success</response>
        /// <response code="401">If user is unauthorized</response>
        [HttpGet]
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
        ///     GET /{songId}
        /// 
        /// </remarks>
        /// <param name="songId">ID of tiked song</param>
        /// <returns>Returns LikedSongVm</returns>
        /// <response code="200">Success</response>
        /// <response code="401">If user is unauthorized</response>
        [HttpGet("{songId}")]
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
        ///     POST /{songId}
        /// 
        /// </remarks>
        /// <param name="songId">ID of song ot like</param>
        /// <returns>Returns string</returns>
        /// <response code="201">Success</response>
        /// <response code="401">If user is unauthorized</response>
        [HttpPost("{songId}")]
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
        ///     DELETE /{songId}
        /// 
        /// </remarks>
        /// <param name="songId">
        /// Id of the song to remove from user favorites 
        /// </param>
        /// <response code="204">Success</response>
        /// <response code="401">If user is unauthorized</response>
        [HttpDelete("{songId}")]
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
