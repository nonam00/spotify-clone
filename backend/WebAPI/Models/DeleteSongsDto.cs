using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public record DeleteSongsDto([Required] List<Guid> SongIds);