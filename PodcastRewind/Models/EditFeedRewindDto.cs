using System.ComponentModel.DataAnnotations;

namespace PodcastRewind.Models;

public class EditFeedRewindDto
{
    public Guid Id { get; init; }

    [Display(Name = "Feed URL")]
    public string FeedUrl { get; init; } = string.Empty;

    [Required]
    [Display(Name = "Episode to set as latest")]
    public string KeyEntryId { get; init; } = string.Empty;

    [Required]
    [Display(Name = "Rewind selected episode to")]
    public DateTime DateOfKeyEntry { get; init; }

    [Required]
    [Display(Name = "Days between episodes")]
    public int Interval { get; init; }
}
