using MediatR;

namespace Application.Users.Queries.GetUserInfo
{
    public class GetUserInfoQuery : IRequest<UserInfo>
    {
        public Guid UserId { get; set; } 
    }
}
