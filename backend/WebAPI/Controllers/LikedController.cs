using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiVersionNeutral]
    [Produces("application/json")]
    [Route("{version:apiVersion}/liked")]
    public class LikedController(IMediator mediator) : BaseController
    {
        private readonly IMediator _mediator = mediator;
    }
}
