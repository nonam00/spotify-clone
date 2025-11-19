using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public record ReorderSongsDto([Required] List<Guid> SongIds);