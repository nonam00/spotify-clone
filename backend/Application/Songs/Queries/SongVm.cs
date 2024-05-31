using Domain;
using Application.Common.Mappings;
using AutoMapper;

namespace Application.Songs.Queries
{
    public class SongVm : IMapWith<Song>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string SongPath { get; set; }
        public string ImagePath { get; set; }

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
