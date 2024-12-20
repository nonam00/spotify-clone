using AutoMapper;

using Domain;
using Application.Common.Mappings;

namespace Application.Songs.Queries
{
    public class SongVm : IMapWith<Song>
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = null!;
        public string Author { get; init; } = null!;
        public string SongPath { get; init; } = null!;
        public string ImagePath { get; init; } = null!;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Song, SongVm>()
                .ForMember(songVm => songVm.Id,
                    opt => opt.MapFrom(song => song.Id))
                .ForMember(songVm => songVm.Title,
                    opt => opt.MapFrom(song => song.Title))
                .ForMember(songVm => songVm.Author,
                    opt => opt.MapFrom(song => song.Author))
                .ForMember(songVm => songVm.SongPath,
                    opt => opt.MapFrom(song => song.SongPath))
                .ForMember(songVm => songVm.ImagePath,
                    opt => opt.MapFrom(song => song.ImagePath));
        }
    }
}
