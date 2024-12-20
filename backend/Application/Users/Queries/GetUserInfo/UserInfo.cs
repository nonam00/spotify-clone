using Application.Common.Mappings;
using AutoMapper;

using Domain;

namespace Application.Users.Queries.GetUserInfo
{
    public class UserInfo : IMapWith<User>
    {
        public string Email { get; init; } = null!;
        public string? FullName { get; init; }
        public string? AvatarUrl { get; init; }
        public string? PaymentMethod { get; init; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserInfo>()
                .ForMember(vm => vm.Email,
                    opt => opt.MapFrom(u => u.Email))
                .ForMember(vm => vm.FullName,
                    opt => opt.MapFrom(u => u.FullName))
                .ForMember(vm => vm.AvatarUrl,
                    opt => opt.MapFrom(u => u.AvatarUrl))
                .ForMember(vm => vm.PaymentMethod,
                    opt => opt.MapFrom(u => u.PaymentMethod));
        }
    }
}
