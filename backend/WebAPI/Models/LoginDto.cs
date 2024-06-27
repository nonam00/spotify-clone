using AutoMapper;
using System.ComponentModel.DataAnnotations;

using Application.Common.Mappings;
using Application.Users.Queries.Login;

namespace WebAPI.Models
{
    public class LoginDto : IMapWith<LoginQuery>
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<LoginDto, LoginQuery>()
                .ForMember(command => command.Email,
                    opt => opt.MapFrom(dto => dto.Email))
                .ForMember(command => command.Password,
                    opt => opt.MapFrom(dto => dto.Password));
        }
    }
}
