using Microsoft.Extensions.Logging;

using Application.Shared.Messaging;
using Application.Songs.Interfaces;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetSongById;

public class GetSongByIdQueryHandler : IQueryHandler<GetSongByIdQuery, SongVm>
{
    private readonly ISongsRepository _songsRepository;
    private readonly ILogger<GetSongByIdQueryHandler> _logger;

    public GetSongByIdQueryHandler(ISongsRepository songsRepository, ILogger<GetSongByIdQueryHandler> logger)
    {
        _songsRepository = songsRepository;
        _logger = logger;
    }

    public async Task<SongVm> Handle(GetSongByIdQuery request, CancellationToken cancellationToken)
    {
        var song = await _songsRepository.GetById(request.SongId, cancellationToken);

        if (song != null)
        {
            return new SongVm(song.Id, song.Title, song.Author, song.SongPath, song.ImagePath);
        }
        
        _logger.LogError("Tried to get song {songId} but it does not exist", request.SongId);
        throw new ArgumentException("Song doesn't exist");
    }
}