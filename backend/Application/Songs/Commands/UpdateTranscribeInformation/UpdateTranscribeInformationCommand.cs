using Domain.Common;
using Application.Shared.Messaging;

namespace Application.Songs.Commands.UpdateTranscribeInformation;

public record UpdateTranscribeInformationCommand(Guid SongId, bool ContainsExplicitContent) : ICommand<Result>;