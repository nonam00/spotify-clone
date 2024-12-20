using AutoMapper;
using System.ComponentModel.DataAnnotations;

using Application.Common.Mappings;
using Application.Users.Commands.CreateUser;

namespace WebAPI.Models
{
    public class RegisterDto : IMapWith<CreateUserCommand>
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<RegisterDto, CreateUserCommand>()
                .ForMember(query => query.Email,
                    opt => opt.MapFrom(dto => dto.Email))
                .ForMember(query => query.Password,
                    opt => opt.MapFrom(dto => dto.Password));
        }
    }
}
