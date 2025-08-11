using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    private IMediator? _mediator;
    protected IMediator Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

    internal Guid UserId => !User.Identity!.IsAuthenticated
        ? Guid.Empty
        : Guid.Parse(User.Claims.First(c => c.Type == "userId").Value);
}