using Application.Common.Mappings;
using AutoMapper;

using Domain;

namespace Application.Users.Queries.GetUserInfo
{
    public class UserInfo : IMapWith<User>
    {
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PaymentMethod { get; set; }

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
