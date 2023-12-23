using System.ComponentModel.DataAnnotations;

namespace PodcastRewind.Models.Dto;

public class CreateFeedRewindDto
{
    [Required]
    public string FeedUrl { get; init; } = string.Empty;

    [Required(ErrorMessage = "One episode must be selected.")]
    [Display(Name = "Episode to set as latest:")]
    public string KeyEntryId { get; init; } = string.Empty;

    [Required(ErrorMessage = "A valid date must be entered.")]
    [DataType(DataType.Date)]
    [Display(Name = "Rewind selected episode to:")]
    public DateTime DateOfKeyEntry { get; init; } = DateTime.Today;

    [Required(ErrorMessage = "An interval for new episodes must be entered.")]
    [Range(1, 365, ErrorMessage = "The interval must be between {1} and {2} days.")]
    [Display(Name = "Days between episodes:")]
    public int Interval { get; init; } = 7;
}
