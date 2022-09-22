namespace PodcastRewind.Models.Entities;

public class FeedRewindInfo
{
    public Guid Id { get; init; }
    public string FeedUrl { get; init; } = string.Empty;
    public string KeyEntryId { get; init; } = string.Empty;
    public DateTime DateOfKeyEntry { get; init; }
    public int Interval { get; init; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public DateTime UpdatedOn { get; set; } = DateTime.Now;
}
