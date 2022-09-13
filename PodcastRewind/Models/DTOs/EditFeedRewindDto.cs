using System.ComponentModel.DataAnnotations;

namespace PodcastRewind.Models.DTOs;

public class EditFeedRewindDto
{
    public Guid Id { get; init; }

    [Required]
    [Display(Name = "Feed URL")]
    public string FeedUrl { get; init; } = string.Empty;

    [Required(ErrorMessage = "One episode must be selected.")]
    [Display(Name = "Episode to set as latest")]
    public string KeyEntryId { get; init; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Rewind selected episode to")]
    public DateTime DateOfKeyEntry { get; init; }

    [Required]
    [Range(1, 365, ErrorMessage = "The interval must be between {1} and {2} days.")]
    [Display(Name = "Days between episodes")]
    public int Interval { get; init; }
}
