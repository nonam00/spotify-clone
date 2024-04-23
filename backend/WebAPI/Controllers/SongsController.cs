using AutoMapper;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

using Application.Songs.Commands.CreateSong;
using Application.Songs.Queries.GetSongList;
using Application.Songs.Queries.GetSongList.GetSongListByTitle;

namespace WebAPI.Controllers
{
    [ApiVersionNeutral]
    [Produces("application/json")]
    [Route("{version:apiVersion}/songs")]
    public class SongsController(IMapper mapper) : BaseController
    {
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Gets search results
        /// </summary>
        /// <remarks>
        /// GET /search/hysteria
        /// </remarks>
        /// <param name="searchString">User search query by song title</param>
        /// <respose code="200">Success</respose>
        /// <returns></returns>
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
    }
}
