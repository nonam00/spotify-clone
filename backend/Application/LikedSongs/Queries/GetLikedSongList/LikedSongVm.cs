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
        public string SongPath { get; set; }
        public string ImagePath { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<LikedSong, LikedSongVm>()
                .ForMember(likedVm => likedVm.Id,
                    opt => opt.MapFrom(liked => liked.Song.Id))
                .ForMember(likedVm => likedVm.Title,
                    opt => opt.MapFrom(liked => liked.Song.Title))
                .ForMember(likedVm => likedVm.Author,
                    opt => opt.MapFrom(liked => liked.Song.Author))
                .ForMember(likedVm => likedVm.SongPath,
                    opt => opt.MapFrom(liked => liked.Song.SongPath))
                .ForMember(command => command.ImagePath,
                    opt => opt.MapFrom(liked => liked.Song.ImagePath));
        }
    }
}
