using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Application.Moderators.Commands.CreateModerator;
using Application.Moderators.Commands.ActivateModerator;
using Application.Moderators.Commands.ActivateUser;
using Application.Moderators.Commands.DeactivateModerator;
using Application.Moderators.Commands.DeactivateUser;
using Application.Moderators.Commands.DeleteSong;
using Application.Moderators.Commands.DeleteSongs;
using Application.Moderators.Commands.PublishSong;
using Application.Moderators.Commands.PublishSongs;
using Application.Moderators.Commands.UnpublishSong;
using Application.Moderators.Models;
using Application.Moderators.Queries.GetModeratorInfo;
using Application.Moderators.Queries.GetModeratorList;
using Application.Moderators.Commands.UpdateModeratorPermissions;
using Application.Songs.Models;
using Application.Songs.Queries.GetUnpublishedSongList;
using Application.Songs.Queries.GetUploadedSongsByUserId;
using Application.Users.Models;
using Application.Users.Queries.GetUserList;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Authorize(Policy = AuthorizationPolicies.ModeratorOnly)]
[Route("{version:apiVersion}/moderators"), ApiVersionNeutral]
public class ModeratorsController : BaseController
{
    private Guid ModeratorId => GetGuidClaim("moderatorId");

    [HttpGet("info")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ModeratorInfo>> GetInfo(CancellationToken cancellationToken)
    {
        var query = new GetModeratorInfoQuery(ModeratorId);
        var result = await Mediator.Send(query, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(new { Detail = result.Error.Description });
    }
    
    [HttpGet, Authorize(Policy = AuthorizationPolicies.CanManageModerators)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ModeratorListVm>> GetModerators(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetModeratorListQuery(), cancellationToken);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }
    
    [HttpPost("register"), Authorize(Policy = AuthorizationPolicies.CanManageModerators)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ModeratorRegister(
        [FromForm] CreateModeratorDto createModeratorDto, CancellationToken cancellationToken)
    {
        var command = new CreateModeratorCommand(
            ModeratorId,
            Email: createModeratorDto.Email, 
            FullName: createModeratorDto.FullName,
            Password: createModeratorDto.Password,
            IsSuper: createModeratorDto.IsSuper);
        var result = await Mediator.Send(command, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Created();
        }

        return BadRequest(new { Detail = result.Error.Description });
    }
    
    [HttpPut("{moderatorToUpdateId:guid}/permissions")]
    [Authorize(Policy = AuthorizationPolicies.CanManageModerators)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateModeratorPermissions(
        Guid moderatorToUpdateId,
        UpdateModeratorPermissionsDto updateModeratorPermissionsDto,
        CancellationToken cancellationToken)
    {
        var command = new UpdateModeratorPermissionsCommand(
            ModeratorId,
            moderatorToUpdateId,
            updateModeratorPermissionsDto.CanManageUsers,
            updateModeratorPermissionsDto.CanManageContent,
            updateModeratorPermissionsDto.CanViewReports,
            updateModeratorPermissionsDto.CanManageModerators);
        
        var result = await Mediator.Send(command, cancellationToken);
        
        if (result.IsSuccess)
        {
            return NoContent();
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }
    
    [HttpPut("{moderatorToActivateId:guid}/activate")]
    [Authorize(Policy = AuthorizationPolicies.CanManageModerators)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ActivateModerator(
        Guid moderatorToActivateId,
        CancellationToken cancellationToken)
    {
        var command = new ActivateModeratorCommand(
            ManagingModeratorId: ModeratorId,
            ModeratorToActivateId: moderatorToActivateId);
        
        var result = await Mediator.Send(command, cancellationToken);
        
        if (result.IsSuccess)
        {
            return NoContent();
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }
    
    [HttpPut("{moderatorToDeactivateId:guid}/deactivate")]
    [Authorize(Policy = AuthorizationPolicies.CanManageModerators)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeactivateModerator(
        Guid moderatorToDeactivateId,
        CancellationToken cancellationToken)
    {
        var command = new DeactivateModeratorCommand(
            ManagingModeratorId: ModeratorId,
            ModeratorToDeactivateId: moderatorToDeactivateId);
        
        var result = await Mediator.Send(command, cancellationToken);
        
        if (result.IsSuccess)
        {
            return NoContent();
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }
    
    [HttpDelete("songs/{songId:guid}"), Authorize(Policy = AuthorizationPolicies.CanManageContent)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteSong(Guid songId, CancellationToken cancellationToken)
    {
        var command = new DeleteSongCommand(ModeratorId, songId);
        var result = await Mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return NoContent();
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }
    
    [HttpDelete("songs"), Authorize(Policy = AuthorizationPolicies.CanManageContent)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteSongs(DeleteSongsDto deleteSongsDto, CancellationToken cancellationToken)
    {
        var command = new DeleteSongsCommand(ModeratorId, deleteSongsDto.SongIds);
        var result = await Mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return NoContent();
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }

    
    [HttpGet("songs/unpublished"), Authorize(Policy = AuthorizationPolicies.CanManageContent)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SongListVm>> GetUnpublishedSongs(CancellationToken cancellationToken)
    {
        var query = new GetUnpublishedSongListQuery();
        var result = await Mediator.Send(query, cancellationToken);
        
        if (result.IsSuccess)
        {
            return result.Value;
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }
    
    [HttpPut("songs/publish/{songId:guid}"), Authorize(Policy = AuthorizationPolicies.CanManageContent)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> PublishSong(Guid songId, CancellationToken cancellationToken)
    {
        var command = new PublishSongCommand(ModeratorId, songId);
        var result = await Mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return NoContent();
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }
    
    
    [HttpPut("songs/publish"), Authorize(Policy = AuthorizationPolicies.CanManageContent)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> PublishSongs(
        PublishSongsDto publishSongsDto,
        CancellationToken cancellationToken)
    {
        var command = new PublishSongsCommand(ModeratorId, publishSongsDto.SongIds);
        var result = await Mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return NoContent();
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }
    
    [HttpPut("songs/unpublish/{songId:guid}"), Authorize(Policy = AuthorizationPolicies.CanManageContent)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UnpublishSong(Guid songId, CancellationToken cancellationToken)
    {
        var command = new UnpublishSongCommand(ModeratorId, songId);
        var result = await Mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return NoContent();
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }
    
    [HttpGet("users"), Authorize(Policy = AuthorizationPolicies.CanManageUsers)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserListVm>> GetUsers(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetUserListQuery(), cancellationToken);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }
    
    [HttpGet("users/{userId:guid}/songs"), Authorize(Policy = AuthorizationPolicies.CanManageUsers)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SongListVm>> GetUserSongs(Guid userId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetUploadedSongsByUserIdQuery(userId), cancellationToken);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }
    
    [HttpPut("users/{userId:guid}/activate"), Authorize(Policy = AuthorizationPolicies.CanManageUsers)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ActivateUser(Guid userId, CancellationToken cancellationToken)
    {
        var command = new ActivateUserCommand(ModeratorId, userId);
        var result = await Mediator.Send(command, cancellationToken);
        
        if (result.IsSuccess)
        {
            return NoContent();
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }
    
    [HttpPut("users/{userId:guid}/deactivate"), Authorize(Policy = AuthorizationPolicies.CanManageUsers)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeactivateUser(Guid userId, CancellationToken cancellationToken)
    {
        var command = new DeactivateUserCommand(ModeratorId, userId);
        var result = await Mediator.Send(command, cancellationToken);
        
        if (result.IsSuccess)
        {
            return NoContent();
        }
        
        return BadRequest(new { Detail = result.Error.Description });
    }
}