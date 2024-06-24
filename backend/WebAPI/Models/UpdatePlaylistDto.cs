using AutoMapper;
using System.ComponentModel.DataAnnotations;

using Application.Playlists.Commands.UpdatePlaylist;
using Application.Common.Mappings;

namespace WebAPI.Models
{
    public class UpdatePlaylistDto : IMapWith<UpdatePlaylistCommand>
    {
        [Required]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImagePath { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<UpdatePlaylistDto, UpdatePlaylistCommand>()
                .ForMember(c => c.Title,
                    opt => opt.MapFrom(dto => dto.Title))
                .ForMember(c => c.Description,
                    opt => opt.MapFrom(dto => dto.Description ?? null))
                .ForMember(c => c.ImagePath,
                    opt => opt.MapFrom(dto => dto.ImagePath ?? null));
        }
    }
}
