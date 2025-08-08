using System.ComponentModel.DataAnnotations;
using Application.Songs.Enums;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Models;

[BindProperties]
public class SearchSongDto
{
    [Required] public string SearchString { get; set; } = null!;
    [Required] public SearchCriteria SearchCriteria { get; set; }
}