﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.LikedSongs.Commands.DeleteLikedSong
{
    public class DeleteLikedSongCommandValidator
        : AbstractValidator<DeleteLikedSongCommand>
    {
        public DeleteLikedSongCommandValidator()
        {
            RuleFor(command => command.UserId).NotEqual(Guid.Empty);
            RuleFor(command => command.SongId).NotEqual(Guid.Empty);
        }
    }
}
