using AutoMapper;

using Domain;
using Application.Common.Mappings;
using Application.Songs.Queries.GetSongList;

namespace Application.LikedSongs.Queries.GetLikedSongList
{
    public class LikedSongVm : IMapWith<LikedSong>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }

        // paths may be needed

        public void Mappings(Profile profile)
        {
            profile.CreateMap<LikedSong, LikedSongVm>()
                .ForMember(likedVm => likedVm.Id,
                    opt => opt.MapFrom(liked => liked.Song.Id))
                .ForMember(likedVm => likedVm.Title,
                    opt => opt.MapFrom(liked => liked.Song.Title))
                .ForMember(likedVm => likedVm.Id,
                    opt => opt.MapFrom(liked => liked.Song.Author));
        }
    }
}
