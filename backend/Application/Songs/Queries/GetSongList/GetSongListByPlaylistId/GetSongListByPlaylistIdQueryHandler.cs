using MediatR;
using Application.Songs.Interfaces;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetSongList.GetSongListByPlaylistId
{
    public class GetSongListByPlaylistIdQueryHandler: IRequestHandler<GetSongListByPlaylistIdQuery, SongListVm>
    {
        private readonly ISongsRepository _songsRepository;

        public GetSongListByPlaylistIdQueryHandler(ISongsRepository songsRepository)
        {
            _songsRepository = songsRepository;
        }

        public async Task<SongListVm> Handle(GetSongListByPlaylistIdQuery request,
            CancellationToken cancellationToken)
        {
            var songs = await _songsRepository.GetListByPlaylistId(
                request.PlaylistId, cancellationToken);
            
            return new SongListVm { Songs = songs };
        }
    }
}
