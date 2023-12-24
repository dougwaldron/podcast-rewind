using PodcastRewind.Models.Entities;

namespace PodcastRewind.TestData;

internal static partial class Data
{
    public static List<FeedRewindInfo> FeedRewindInfoData { get; } =
    [
        new FeedRewindInfo
        {
            Id = new Guid("10000000-0000-0000-0000-000000000001"),
            FeedUrl = "https://example.net",
            KeyEntryId = new Guid("20000000-0000-0000-0000-000000000001").ToString(),
            DateOfKeyEntry = DateTime.Today.AddDays(-7),
            Interval = 1,
        }
    ];
}
