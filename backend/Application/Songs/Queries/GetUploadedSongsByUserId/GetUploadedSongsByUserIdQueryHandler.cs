using Microsoft.Extensions.Logging;

using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Interfaces;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetUploadedSongsByUserId;

public class GetUploadedSongsByUserIdQueryHandler : IQueryHandler<GetUploadedSongsByUserIdQuery, Result<SongListVm>>
{
    private readonly ISongsRepository _songsRepository;
    private readonly ILogger<GetUploadedSongsByUserIdQueryHandler> _logger;

    public GetUploadedSongsByUserIdQueryHandler(
        ISongsRepository songsRepository,
        ILogger<GetUploadedSongsByUserIdQueryHandler> logger)
    {
        _songsRepository = songsRepository;
        _logger = logger;
    }

    public async Task<Result<SongListVm>> Handle(GetUploadedSongsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var songs = await _songsRepository.GetUploadedByUserId(request.UserId, cancellationToken);
        return Result<SongListVm>.Success(new SongListVm(songs));
    }
}

