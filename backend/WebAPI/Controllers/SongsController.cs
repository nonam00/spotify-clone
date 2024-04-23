using AutoMapper;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

using Application.Songs.Commands.CreateSong;
using Application.Songs.Queries.GetSongList;
using Application.Songs.Queries.GetSongList.GetSongListByTitle;

using WebAPI.Models;

namespace WebAPI.Controllers
{
    [ApiVersionNeutral]
    [Produces("application/json")]
    [Route("{version:apiVersion}/songs")]
    public class SongsController(IMapper mapper) : BaseController
    {
        private readonly IMapper _mapper = mapper;

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
        /// <respose code="200">Success</respose>
        /// <response code="401">If the user is unathorized</response>
        //[Authorize]
        [HttpPost("post")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Guid>> AddNewSong([FromBody] CreateSongDto createSongDto)
        {
            var command = _mapper.Map<CreateSongCommand>(createSongDto);
            command.UserId = UserId;
            var songId = await Mediator.Send(command);
            return Ok(songId);
        }
    }
}
