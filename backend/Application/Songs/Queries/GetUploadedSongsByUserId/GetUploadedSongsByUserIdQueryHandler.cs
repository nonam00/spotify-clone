using Application.Shared.Data;
using Application.Shared.Messaging;
using Application.Songs.Interfaces;
using Application.Songs.Models;

namespace Application.Songs.Queries.GetUploadedSongsByUserId;

public class GetUploadedSongsByUserIdQueryHandler : IQueryHandler<GetUploadedSongsByUserIdQuery, Result<SongListVm>>
{
    private readonly ISongsRepository _songsRepository;

    public GetUploadedSongsByUserIdQueryHandler(ISongsRepository songsRepository)
    {
        _songsRepository = songsRepository;
    }

    public async Task<Result<SongListVm>> Handle(GetUploadedSongsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var songs = await _songsRepository.GetUploadedByUserId(request.UserId, cancellationToken);
        return Result<SongListVm>.Success(new SongListVm(songs));
    }
}

