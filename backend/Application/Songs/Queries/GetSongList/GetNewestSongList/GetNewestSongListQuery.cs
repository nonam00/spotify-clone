using MediatR;

using Application.Songs.Models;

namespace Application.Songs.Queries.GetSongList.GetNewestSongList;

public class GetNewestSongListQuery : IRequest<SongListVm>;