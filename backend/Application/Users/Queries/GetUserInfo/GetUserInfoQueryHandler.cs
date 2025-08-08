using AutoMapper;
using MediatR;

using Application.Users.Interfaces;
using Application.Users.Models;

namespace Application.Users.Queries.GetUserInfo;

public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, UserInfo>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IMapper _mapper;

    public GetUserInfoQueryHandler(IUsersRepository usersRepository, IMapper mapper)
    {
        _usersRepository = usersRepository;
        _mapper = mapper;
    }

    public async Task<UserInfo> Handle(GetUserInfoQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository.Get(request.UserId, cancellationToken)
                   ?? throw new Exception("Invalid current user id");

        var userVm = _mapper.Map<UserInfo>(user);

        return userVm;
    }
}