﻿using FluentValidation;

namespace Application.LikedSongs.Queries.GetLikedSongList.GetLikedSongList;

public class GetLikedSongListQueryValidator : AbstractValidator<GetLikedSongListQuery>
{
    public GetLikedSongListQueryValidator()
    {
        RuleFor(command => command.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("User ID is required")
            .WithErrorCode("400");   
    }
}