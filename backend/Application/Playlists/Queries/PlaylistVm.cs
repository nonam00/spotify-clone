using AutoMapper;

using Domain;
using Application.Common.Mappings;

namespace Application.Playlists.Queries
{
    public class PlaylistVm : IMapWith<Playlist>  
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Playlist, PlaylistVm>() 
                .ForMember(vm => vm.Id,
                    opt => opt.MapFrom(p => p.Id))
                .ForMember(vm => vm.Title,
                    opt => opt.MapFrom(p => p.Title))
                .ForMember(vm => vm.Description,
                    opt => opt.MapFrom(p => p.Description))
                .ForMember(vm => vm.ImagePath,
                    opt => opt.MapFrom(p => p.ImagePath));
        }     
    }
}
