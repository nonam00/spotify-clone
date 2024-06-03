using AutoMapper;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Application.Songs.Commands.CreateSong;
using Application.Songs.Queries.GetSongList;
using Application.Songs.Queries.GetSongList.GetSongListByTitle;
using Application.Songs.Queries.GetSongList.GetAllSongs;
using Application.Songs.Queries.GetSongById;
using Application.Songs.Queries;

using WebAPI.Models;

namespace WebAPI.Controllers
{
    [ApiVersionNeutral]
    [Produces("application/json")]
    [Route("{version:apiVersion}/songs")]
    public class SongsController(IMapper mapper) : BaseController
    {
        private readonly IMapper _mapper = mapper;

        [HttpGet("get/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<SongListVm>> GetAllSongs()
        {
            var query = new GetAllSongsQuery();
            var vm = await Mediator.Send(query);
            return Ok(vm);
        }

        [HttpGet("get/{songId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<SongVm>> GetSongById(Guid songId)
        {
            var query = new GetSongByIdQuery
            {
                SongId = songId
            };
            var vm = await Mediator.Send(query);
            return Ok(vm);
        }

        /// <summary>
        /// Gets songs satisfying the search request
        /// </summary>
        /// <remarks>
        /// GET /search/hysteria
        /// </remarks>
        /// <param name="searchString">User search query by song title</param>
        /// <returns>Returns SongListVm</returns>
        /// <respose code="200">Success</respose>
        [HttpGet("search/{searchString}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<SongListVm>> GetSongListByTitle(string searchString)
        {
            var query = new GetSongListByTitleQuery
            {
                SearchString = searchString
            };
            var vm = await Mediator.Send(query);
            return Ok(vm);
        }
        /// <summary>
        /// Creates new song 
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// POST /post
        /// {
        ///     
        /// }
        /// </remarks>
        /// <param name="createSongDto">createSongDto object</param>
        /// <returns>Returns id (guid)</returns>
        /// <respose code="201">Success</respose>
        /// <response code="401">If the user is unauthorized</response>
        [Authorize]
        [HttpPost("post")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Guid>> UploadNewSong([FromBody] CreateSongDto createSongDto)
        {
            var command = _mapper.Map<CreateSongCommand>(createSongDto);
            command.UserId = UserId;
            var songId = await Mediator.Send(command);
            return Ok(songId);
        }
    }
}
