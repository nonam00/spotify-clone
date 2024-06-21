using AutoMapper;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Application.Playlists.Queries.GetPlaylistById;
using Application.Playlists.Queries.GetPlaylistListByUserId;
using Application.Playlists.Queries;
using Application.Playlists.Commands.CreatePlaylist;
using Application.Playlists.Commands.UpdatePlaylist;
using Application.Playlists.Commands.DeletePlaylist;

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
        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PlaylistVm>> GetPlaylist(Guid id)
        {
           var query = new GetPlaylistByIdQuery
           {
              Id = id
           };
           var vm = await Mediator.Send(query);
           return vm;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PlaylistListVm>> GetPlaylistList()
        {
            var query = new GetPlaylistListByUserIdQuery
            {
                UserId = UserId
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

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdatePlaylist(Guid id,
            [FromBody] UpdatePlaylistDto updatePlaylistDto)
        {
            var command = _mapper.Map<UpdatePlaylistCommand>(updatePlaylistDto);
            command.Id = id;
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeletePlaylist(Guid id)
        {
            var command = new DeletePlaylistCommand
            {
                Id = id
            };
            await Mediator.Send(command);
            return NoContent();
        }
    }
}

