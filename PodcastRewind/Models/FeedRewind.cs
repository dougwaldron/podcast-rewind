namespace PodcastRewind.Models;

public class FeedRewind
{
    public Guid Id { get; init; }
    public string FeedUrl { get; init; } = string.Empty;
    public string KeyEntryId { get; init; } = string.Empty;
    public DateTime DateOfKeyEntry { get; init; }
    public int Interval { get; init; }
}
