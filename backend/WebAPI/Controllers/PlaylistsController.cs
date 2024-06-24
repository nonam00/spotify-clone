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

using Application.PlaylistSongs.Queries.CheckPlaylistSong;
using Application.PlaylistSongs.Commands.CreatePlaylistSong;
using Application.PlaylistSongs.Commands.DeletePlaylistSong;

using Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListByPlaylistId;
using Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongListBySearchStringAndPlaylistId;
using Application.LikedSongs.Queries.GetLikedSongList;

using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Authorize]
    //[AutoValidateAntiforgeryToken]
    [ApiVersionNeutral]
    [Produces("application/json")]
    [Route("{version:apiVersion}/playlists")]
    public class PlaylistsController(IMapper mapper) : BaseController
    {
        private readonly IMapper _mapper = mapper;
        
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

        [HttpPut("{playlistId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdatePlaylist(Guid playlistId,
            [FromBody] UpdatePlaylistDto updatePlaylistDto)
        {
            var command = _mapper.Map<UpdatePlaylistCommand>(updatePlaylistDto);
            command.Id = playlistId;
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{playlistId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeletePlaylist(Guid playlistId)
        {
            var command = new DeletePlaylistCommand
            {
                Id = playlistId
            };
            await Mediator.Send(command);
            return NoContent();
        }

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
        
        [HttpGet("{playlistId}/songs/{songId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<bool>> CheckPlaylistSong(Guid playlistId, Guid songId)
        {
            var query = new CheckPlaylistSongQuery
            {
                PlaylistId = playlistId,
                SongId = songId
            };
            var check = await Mediator.Send(query);
            return check;
        }

        [HttpPost("{playlistId}/songs/{songId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<string>> CreatePlaylistSong(Guid playlistId, Guid songId)
        {
            var command = new CreatePlaylistSongCommand
            {
                PlaylistId = playlistId,
                SongId = songId,
            };
            var ids = await Mediator.Send(command);
            return ids;
        }

        [HttpDelete("{playlistId}/songs/{songId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeletePlaylistSong(Guid playlistId, Guid songId)
        {
            var command = new DeletePlaylistSongCommand
            {
                PlaylistId = playlistId,
                SongId = songId
            };
            await Mediator.Send(command);
            return NoContent();
        }

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

