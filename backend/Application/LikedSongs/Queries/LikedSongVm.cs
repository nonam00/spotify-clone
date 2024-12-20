using AutoMapper;

using Application.Common.Mappings;
using Domain;

namespace Application.LikedSongs.Queries
{
    public class LikedSongVm : IMapWith<LikedSong>
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = null!;
        public string Author { get; init; } = null!;
        public string SongPath { get; init; } = null!;
        public string ImagePath { get; init; } = null!;

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
