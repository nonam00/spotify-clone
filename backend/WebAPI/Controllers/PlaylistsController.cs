using AutoMapper;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Application.Playlists.Queries.GetPlaylistById;
using Application.Playlists.Queries.GetPlaylistList;
using Application.Playlists.Queries.GetPlaylistList.GetFullPlaylistList;
using Application.Playlists.Queries.GetPlaylistList.GetPlaylistListByCount;
using Application.Playlists.Queries;
using Application.Playlists.Commands.CreatePlaylist;
using Application.Playlists.Commands.UpdatePlaylist;
using Application.Playlists.Commands.DeletePlaylist;

using Application.Songs.Queries.GetSongList.GetSongListByPlaylistId;
using Application.Songs.Queries.GetSongList;

using Application.PlaylistSongs.Commands.CreatePlaylistSong;
using Application.PlaylistSongs.Commands.DeletePlaylistSong;

using Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListByPlaylistId;
using Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListBySearchStringAndPlaylistId;
using Application.LikedSongs.Queries.GetLikedSongList;

using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiVersionNeutral]
    [Produces("application/json")]
    [Route("{version:apiVersion}/playlists")]
    public class PlaylistsController(IMapper mapper) : BaseController
    {
        private readonly IMapper _mapper = mapper;
        
        /// <summary>
        /// Gets certain user playlist 
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     GET /{playlistId}
        /// 
        /// </remarks>
        /// <param name="playlistId">ID of playlist</param>
        /// <returns>Returns PlaylistVm</returns>
        /// <response code="201">Success</response>
        /// <response code="401">If user is unauthorized</response>
        [HttpGet("{playlistId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PlaylistVm>> GetPlaylist(Guid playlistId)
        {
           var query = new GetPlaylistByIdQuery
           {
              Id = playlistId
           };
           var vm = await Mediator.Send(query);
           return vm;
        }

        /// <summary>
        /// Gets list of user playlist 
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     GET /
        /// 
        /// </remarks>
        /// <returns>Returns PlaylistListVm</returns>
        /// <response code="201">Success</response>
        /// <response code="401">If user is unauthorized</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PlaylistListVm>> GetPlaylistList()
        {
            var query = new GetFullPlaylistListQuery
            {
                UserId = UserId
            };
            var vm = await Mediator.Send(query);
            return vm;
        }

        /// <summary>
        /// Gets certain quantity of user playlists
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     GET /certain/{count}
        /// 
        /// </remarks>
        /// <param name="count">Count of playlist</param>
        /// <returns>Returns PlaylistListVm</returns>
        /// <response code="201">Success</response>
        /// <response code="401">If user is unauthorized</response>
        [HttpGet("certain/{count}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PlaylistListVm>> GetPlaylistListByCount(int count)
        {
            var query = new GetPlaylistListByCountQuery
            {
                UserId = UserId,
                Count = count
            };
            var vm = await Mediator.Send(query);
            return vm;
        }

        /// <summary>
        /// Creates new user playlist 
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///     POST /
        ///
        /// </remarks>
        /// <returns>Returns Guid</returns>
        /// <response code="201">Success</response>
        /// <response code="401">If user is unauthorized</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Guid>> CreatePlaylist()
        {
            var command = new CreatePlaylistCommand
            {
                UserId = UserId
            };
            var id = await Mediator.Send(command);
            return id;
        }

        /// <summary>
        /// Updates playlist information
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///   PUT /{playlistId}
        ///   {
        ///       title: "New Playlist"
        ///       description: "Description",
        ///       imagePath: "image.png"
        ///   }
        ///
        /// </remarks>
        /// <param name="playlistId">
        ///   id of the playlist that needs to be updated
        /// </param>
        /// <param name="updatePlaylistDto">object with new info</param>
        /// <response code="204">Success</response>
        /// <response code="401">If the user is unauthorized</response>
        [HttpPut("{playlistId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdatePlaylist(Guid playlistId,
            UpdatePlaylistDto updatePlaylistDto)
        {
            var command = _mapper.Map<UpdatePlaylistCommand>(updatePlaylistDto);
            command.UserId = UserId;
            command.PlaylistId = playlistId;
            await Mediator.Send(command);
            return NoContent();
        }
        
        /// <summary>
        /// Deletes user playlist
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /{playlistId}
        ///
        /// </remarks>
        /// <param name="playlistId">
        /// ID of the playlist that needs to be deleted
        /// </param>
        /// <response code="204">Success</response>
        /// <response code="401">If the user is unauthorized</response>
        [HttpDelete("{playlistId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeletePlaylist(Guid playlistId)
        {
            var command = new DeletePlaylistCommand
            {
                UserId = UserId,
                PlaylistId = playlistId
            };
            await Mediator.Send(command);
            return NoContent();
        }
        
        /// <summary>
        /// Gets songs from user playlist
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /{playlistId}/songs
        ///
        /// </remarks>
        /// <param name="playlistId">
        /// ID of the playlist from which the songs will be gotten
        /// </param>
        /// <returns>Returns SongListVm</returns>
        /// <response code="204">Success</response>
        /// <response code="401">If the user is unauthorized</response>
        [HttpGet("{playlistId}/songs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<SongListVm>> GetPlaylistSongs(Guid playlistId)
        {
            var query = new GetSongListByPlaylistIdQuery
            {
                PlaylistId = playlistId
            };
            var vm = await Mediator.Send(query);
            return vm;
        }

        /// <summary>
        /// Adds the song into the playlist
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///   POST /{playlistId}/songs/{songId}
        ///
        /// </remarks>
        /// <param name="playlistId">
        /// ID of the playlist to which the song is adding
        /// </param>
        /// <param name="songId">
        /// ID of the song which is adding to the playlist
        /// </param>
        /// <returns>Returns db key of created relation</returns>
        /// <response code="201">Success</response>
        /// <response code="401">If the user is unauthorized</response>
        [HttpPost("{playlistId}/songs/{songId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<string>> CreatePlaylistSong(Guid playlistId, Guid songId)
        {
            var command = new CreatePlaylistSongCommand
            {
                UserId = UserId,
                PlaylistId = playlistId,
                SongId = songId,
            };
            var ids = await Mediator.Send(command);
            return ids;
        }

        /// <summary>
        /// Removes the song from the playlist
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///   DELETE /{playlistId}/songs/{songId}
        ///
        /// </remarks>
        /// <param name="playlistId">
        /// ID of the playlist from which the song is removing
        /// </param>
        /// <param name="songId">
        /// ID of the song which is removing from the playlist
        /// </param>
        /// <response code="204">Success</response>
        /// <response code="401">If the user is unauthorized</response>
        [HttpDelete("{playlistId}/songs/{songId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeletePlaylistSong(Guid playlistId, Guid songId)
        {
            var command = new DeletePlaylistSongCommand
            {
                UserId = UserId,
                PlaylistId = playlistId,
                SongId = songId
            };
            await Mediator.Send(command);
            return NoContent();
        }
        
        /// <summary>
        /// Gets user favorite songs which are not in the playlist
        /// </summary>
        /// <remarks>
        ///     
        ///     GET /{playlistId}/liked/
        ///
        /// </remarks>
        /// <param name="playlistId">
        /// ID of the playlist 
        /// </param>
        /// <returns>Returns LikedSongListVm</returns>
        /// <response code="200">Success</response>
        /// <response code="401">If the user is unauthorized</response>
        [HttpGet("{playlistId}/liked")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LikedSongListVm>> GetLikedSongs(Guid playlistId)
        {
            var query = new GetLikedSongListByPlaylistIdQuery
            {
                UserId = UserId,
                PlaylistId = playlistId
            };
            var vm = await Mediator.Send(query);
            return vm;
        }
        
        /// <summary>
        /// Gets user favorite songs which are not in the playlist by search string
        /// </summary>
        /// <remarks>
        /// 
        ///     GET /{playlistId}/liked/{searchString}
        ///
        /// </remarks>
        /// <param name="playlistId">
        /// ID of the playlist
        /// </param>
        /// <param name="searchString">
        /// User search request
        /// </param>
        /// <returns>Returns LikedSongListVm></returns>
        /// <response code="200">Success</response>
        /// <response code="401">If the user is unauthorized</response>
        [HttpGet("{playlistId}/liked/{searchString}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LikedSongListVm>> GetLikedSongsBySearchString(
            Guid playlistId, string searchString)
        {
            var query = new GetLikedSongListBySearchStringAndPlaylistIdQuery
            {
                UserId = UserId,
                PlaylistId = playlistId,
                SearchString = searchString
            };
            var vm = await Mediator.Send(query);
            return vm;
        }
    }
}

