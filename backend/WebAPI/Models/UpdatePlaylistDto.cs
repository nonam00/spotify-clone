using AutoMapper;

using Application.Playlists.Commands.UpdatePlaylist;
using Application.Common.Mappings;

namespace WebAPI.Models
{
    public class UpdatePlaylistDto : IMapWith<UpdatePlaylistCommand>
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<UpdatePlaylistCommand, UpdatePlaylistDto>()
                .ForMember(dto => dto.Title,
                    opt => opt.MapFrom(c => c.Title))
                .ForMember(dto => dto.Description,
                    opt => opt.MapFrom(c => c.Description))
                .ForMember(dto => dto.ImagePath,
                    opt => opt.MapFrom(c => c.ImagePath));
        }
    }
}
