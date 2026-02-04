using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Songs.Commands.DeleteMarkedForDeletionSongs;

public class DeleteMarkedForDeletionSongsCommand : ICommand<Result>;