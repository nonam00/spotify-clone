using AutoMapper;
using System.ComponentModel.DataAnnotations;

using Application.Common.Mappings;
using Application.Songs.Commands.CreateSong;

namespace WebAPI.Models
{
    public class CreateSongDto : IMapWith<CreateSongCommand>
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string SongPath { get; set; }
        [Required]
        public string ImagePath { get; set; }
        
        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateSongDto, CreateSongCommand>()
                .ForMember(command => command.Title,
                    opt => opt.MapFrom(dto => dto.Title))
                .ForMember(command => command.Author,
                    opt => opt.MapFrom(dto => dto.Author))
                .ForMember(command => command.SongPath,
                    opt => opt.MapFrom(dto => dto.SongPath))
                .ForMember(command => command.ImagePath,
                    opt => opt.MapFrom(dto => dto.ImagePath));
        }
    }
}
