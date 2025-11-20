using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public record PublishSongsDto([Required] List<Guid> SongIds);