using AutoMapper;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Application.LikedSongs.Queries.GetLikedSongList;
using Application.LikedSongs.Queries.GetLikedSong;
using Application.LikedSongs.Commands.CreateLikedSong;
using Application.LikedSongs.Commands.DeleteLikedSong;

using Application.LikedSongs.Queries;

namespace WebAPI.Controllers
{
    [ApiVersionNeutral]
    [Produces("application/json")]
    [Route("{version:apiVersion}/liked")]
    [Authorize]
    public class LikedController(IMapper mapper) : BaseController
    {
        private readonly IMapper _mapper = mapper;

        // TODO: documentation
        [HttpGet("get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LikedSongListVm>> GetLikedSongList()
        {
            var query = new GetLikedSongListQuery
            {
                UserId = UserId
            };
            var vm = await Mediator.Send(query);
            return Ok(vm);
        }

        [HttpGet("get/{songId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LikedSongVm?>> GetLikedSong(Guid songId)
        {
            var query = new GetLikedSongQuery()
            {
                UserId = UserId,
                SongId = songId
            };
            var vm = await Mediator.Send(query);
            return vm;
        }

        // TODO: documentation
        [HttpPost("like/{songId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateLiked(Guid songId)
        {
            var command = new CreateLikedSongCommand
            {
                UserId = UserId,
                SongId = songId
            };
            var likedId = await Mediator.Send(command);
            return Ok(likedId);
        }

        // TODO: documentation
        [HttpDelete("delete/{songId}")]
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
