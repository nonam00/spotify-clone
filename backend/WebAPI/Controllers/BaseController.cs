using Microsoft.AspNetCore.Mvc;

using Application.Shared.Messaging;

namespace WebAPI.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected IMediator Mediator =>
        field ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
    
    protected Guid GetGuidClaim(string claimType)
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Guid.Empty;
        }

        var claim = User.Claims.FirstOrDefault(c => c.Type == claimType);
        
        return claim is not null && Guid.TryParse(claim.Value, out var value)
            ? value
            : Guid.Empty;
    }
}