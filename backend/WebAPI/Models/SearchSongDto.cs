using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

using Application.Songs.Enums;

namespace WebAPI.Models;

[BindProperties]
public class SearchSongDto
{
    [Required] public string SearchString { get; init; } = null!;
    [Required] public SearchCriteria SearchCriteria { get; init; }
}