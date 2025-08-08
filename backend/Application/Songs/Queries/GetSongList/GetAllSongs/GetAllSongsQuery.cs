using MediatR;

using Application.Songs.Models;

namespace Application.Songs.Queries.GetSongList.GetAllSongs;

public class GetAllSongsQuery : IRequest<SongListVm>;