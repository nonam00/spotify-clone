using Domain;
using Application.Common.Mappings;
using AutoMapper;

namespace Application.Songs.Queries.GetSongList
{
    public class SongVm : IMapWith<Song>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }

        // paths may be needed

        public void Mappings(Profile profile)
        {
            profile.CreateMap<Song, SongVm>()
                .ForMember(songVm => songVm.Id,
                    opt => opt.MapFrom(song => song.Id))
                .ForMember(songVm => songVm.Title,
                    opt => opt.MapFrom(song => song.Title))
                .ForMember(songVm => songVm.Id,
                    opt => opt.MapFrom(song => song.Author));
        }
    }
}
