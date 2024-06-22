﻿using Application.Interfaces;
using MediatR;

using Domain;

namespace Application.LikedSongs.Commands.CreateLikedSong
{
    public class CreateLikedSongCommandHandler(ISongsDbContext dbContext)
        : IRequestHandler<CreateLikedSongCommand, Guid>
    {
        private readonly ISongsDbContext _dbContext = dbContext;

        public async Task<Guid> Handle(CreateLikedSongCommand request,
            CancellationToken cancellationToken)
        {
            var likedSong = new LikedSong
            {
                UserId = request.UserId,
                SongId = request.SongId,
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.LikedSongs.AddAsync(likedSong, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            // TODO: replace with key
            return Guid.NewGuid();
        }
    }
}
