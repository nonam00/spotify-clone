using AutoMapper;
using MediatR;

using Application.Users.Interfaces;
using Application.Users.Models;

namespace Application.Users.Queries.GetUserInfo;

public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, UserInfo>
{
    private readonly IUsersRepository _usersRepository;

    public GetUserInfoQueryHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<UserInfo> Handle(GetUserInfoQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetById(request.UserId, cancellationToken)
                   ?? throw new Exception("Invalid current user id");

        var userVm = new UserInfo
        {
            Email = user.Email,
            FullName = user.FullName
        };

        return userVm;
    }
}