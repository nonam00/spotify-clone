using Application.Interfaces;
using Application.Songs.Enums;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Songs.Queries.GetSongList.GetSongListBySearch;

public class GetSongListBySearchQueryHandler(ISongsDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetSongListBySearchQuery, SongListVm>
{
    private readonly ISongsDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;
    
    public async Task<SongListVm> Handle(GetSongListBySearchQuery request,
        CancellationToken cancellationToken)
    {
        var songs = _dbContext.Songs.AsNoTracking();

        switch (request.SearchCriteria)
        {
            case SearchCriteria.Any:
                songs = songs
                    .Where(song =>
                        EF.Functions.TrigramsSimilarity(song.Title, request.SearchString) > 0.1 ||
                        EF.Functions.TrigramsSimilarity(song.Author, request.SearchString) > 0.1)
                    .OrderBy(song => EF.Functions.TrigramsSimilarityDistance(song.Title, request.SearchString))
                    .ThenBy(song => EF.Functions.TrigramsSimilarityDistance(song.Author, request.SearchString))
                    .ThenByDescending(song => song.CreatedAt);
                break;
            case SearchCriteria.Title:
                songs = songs
                    .Where(song => EF.Functions.TrigramsSimilarity(song.Title, request.SearchString) > 0.1)
                    .OrderBy(song => EF.Functions.TrigramsSimilarityDistance(song.Title, request.SearchString))
                    .ThenByDescending(song => song.CreatedAt);
                break;
            case SearchCriteria.Author:
                songs = songs
                    .Where(song => EF.Functions.TrigramsSimilarity(song.Author, request.SearchString) > 0.1)
                    .OrderBy(song => EF.Functions.TrigramsSimilarityDistance(song.Author, request.SearchString))
                    .ThenByDescending(song => song.CreatedAt);

                break;
            default:
                throw new Exception("Invalid search criteria");
        }
        
        var result = await songs
            .ProjectTo<SongVm>(_mapper.ConfigurationProvider)
            .Take(50)
            .ToListAsync(cancellationToken);

        return new SongListVm { Songs = result };
    }
}