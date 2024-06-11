using AutoMapper;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Application.Songs.Commands.CreateSong;
using Application.Songs.Queries.GetSongList;
using Application.Songs.Queries.GetSongList.GetSongListByAny;
using Application.Songs.Queries.GetSongList.GetSongListByTitle;
using Application.Songs.Queries.GetSongList.GetSongListByAuthor;
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

        /// <summary>
        /// Gets ALL songs data
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /get/all
        /// 
        /// </remarks>
        /// <returns>Returns SongListVm</returns>
        /// <response code="200">Success</response>
        [HttpGet("get/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<SongListVm>> GetAllSongs()
        {
            var query = new GetAllSongsQuery();
            var vm = await Mediator.Send(query);
            return Ok(vm);
        }

        /// <summary>
        /// Gets certain song by ID
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /get/{songId}
        /// 
        /// </remarks>
        /// <param name="songId">Song ID</param>
        /// <returns>Returns SongVm</returns>
        /// <response code="200">Success</response>
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
        /// Sample request:
        /// 
        ///     GET /search/hysteria
        /// 
        /// </remarks>
        /// <param name="searchString">User search query by song title and author</param>
        /// <returns>Returns SongListVm</returns>
        /// <response code="200">Success</response>

        [HttpGet("search/{searchString}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<SongListVm>> GetSongListByAnyInfo(string searchString)
        {
            var query = new GetSongListByAnyQuery
            {
                SearchString = searchString
            };
            var vm = await Mediator.Send(query);
            return Ok(vm);
        }


        /// <summary>
        /// Gets songs satisfying the search request
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /search/title/hysteria
        /// 
        /// </remarks>
        /// <param name="searchString">User search query by song title</param>
        /// <returns>Returns SongListVm</returns>
        /// <response code="200">Success</response>
        [HttpGet("search/title/{searchString}")]
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
        /// Gets songs satisfying the search request
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /search/author/muse
        /// 
        /// </remarks>
        /// <param name="searchString">User search query by song author</param>
        /// <returns>Returns SongListVm</returns>
        /// <response code="200">Success</response>
        [HttpGet("search/author/{searchString}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<SongListVm>> GetSongListByAuthor(string searchString)
        {
            var query = new GetSongListByAuthorQuery
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
        /// 
        ///     POST /post
        ///     {
        ///         title: "Hysteria"
        ///         author: "Muse"
        ///         songPath: "hysteria.flac"
        ///         imagePath: "absolution.jpeg"
        ///     }
        ///     
        /// </remarks>
        /// <param name="createSongDto">createSongDto object</param>
        /// <returns>Returns created song ID</returns>
        /// <response code="201">Success</response>
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
