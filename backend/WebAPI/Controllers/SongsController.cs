using AutoMapper;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiVersionNeutral]
    [Produces("application/json")]
    [Route("{version:apiVersion}/songs")]
    public class SongsController(IMapper mapper) : BaseController
    {
        private readonly IMapper _mapper = mapper;  
    }
}
