using AutoMapper;
using System.ComponentModel.DataAnnotations;

using Application.Common.Mappings;
using Application.LikedSongs.Commands.CreateLikedSong;

namespace WebAPI.Models
{
    public class CreateLikedSongDto : IMapWith<CreateLikedSongCommand>
    {
        [Required]
        public Guid SongId { get; set; }
        
        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateLikedSongDto, CreateLikedSongCommand>()
                .ForMember(command => command.SongId,
                    opt => opt.MapFrom(dto => dto.SongId));
        }
    }

}
