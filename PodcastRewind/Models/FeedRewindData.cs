using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using PodcastRewind.Models.Entities;

namespace PodcastRewind.Models;

public class FeedRewindData(FeedRewindInfo feedRewindInfo, SyndicationFeed originalFeed, string? feedPageLink = null)
{
    public const string FeedMimeType = "text/xml; charset=utf-8";

    private readonly Uri? _feedPageUri = feedPageLink is null ? null : new Uri(feedPageLink);

    public Uri? OriginalFeedLink { get; } =
        originalFeed.Links.FirstOrDefault(link => link.RelationshipType == "alternate")?.Uri;

    public string FeedTitle { get; } = originalFeed.Title.Text;
    private SyndicationFeed? RewoundFeed { get; set; }
    private DateTimeOffset? MostRecentRewoundFeedEntryDate { get; set; }
    private SyndicationFeed? ScheduledFeed { get; set; }
    private List<SyndicationItem> AllRescheduledFeedItems { get; set; } = [];

    public SyndicationFeed? GetRewoundFeed()
    {
        if (RewoundFeed is null) LoadRewoundFeed();
        return RewoundFeed;
    }

    public SyndicationFeed? GetScheduledFeed()
    {
        if (ScheduledFeed is null) LoadUpcomingFeed();
        return ScheduledFeed;
    }

    public SyndicationFeed GetOriginalFeed() => originalFeed;
    public FeedRewindInfo GetFeedRewindInfo() => feedRewindInfo;

    public DateTimeOffset GetLastModifiedDate()
    {
        if (MostRecentRewoundFeedEntryDate is null) LoadRewoundFeed();
        return MostRecentRewoundFeedEntryDate ?? DateTimeOffset.Now;
    }

    public string GetETag() =>
        $"\"{Convert.ToBase64String(Encoding.UTF8.GetBytes(GetLastModifiedDate().ToString()))}\"";

    private void LoadRewoundFeed()
    {
        if (string.IsNullOrEmpty(feedRewindInfo.FeedUrl)) return;
        RewoundFeed = originalFeed.Clone(true);

        AllRescheduledFeedItems = RewoundFeed.Items.OrderBy(item => item.PublishDate).ToList();
        var feedItemsCount = AllRescheduledFeedItems.Count;
        var keyIndex = AllRescheduledFeedItems.FindIndex(item => item.Id == feedRewindInfo.KeyEntryId);
        var dateOfFirstEntry = DateTime.SpecifyKind(
            feedRewindInfo.DateOfKeyEntry.AddDays(-feedRewindInfo.Interval * keyIndex),
            DateTimeKind.Unspecified);

        for (var i = 0; i < feedItemsCount; i++)
        {
            var feedItem = AllRescheduledFeedItems[i];
            var originalPubDate = feedItem.PublishDate;
            feedItem.PublishDate = new DateTimeOffset(dateOfFirstEntry, originalPubDate.Offset)
                .Add(originalPubDate.TimeOfDay).AddDays(feedRewindInfo.Interval * i);
            feedItem.Summary = new TextSyndicationContent(
                $"[⏪\u2009Originally published {originalPubDate:MMMM d, yyyy}.] {feedItem.Summary.Text}");
        }

        RewoundFeed.Items = AllRescheduledFeedItems.Where(item => item.PublishDate <= DateTimeOffset.Now)
            .OrderByDescending(item => item.PublishDate);
        if (AllRescheduledFeedItems.Exists(item => item.PublishDate <= DateTimeOffset.Now))
            MostRecentRewoundFeedEntryDate = RewoundFeed.Items.First().PublishDate;

        RewoundFeed.Title = new TextSyndicationContent($"⏪: {RewoundFeed.Title.Text}");
        if (_feedPageUri is not null)
            RewoundFeed.Links.Insert(0, SyndicationLink.CreateAlternateLink(_feedPageUri, "text/html"));

        var newDescriptionKind = originalFeed.Description.Type switch
        {
            "html" => TextSyndicationContentKind.Html,
            "xhtml" => TextSyndicationContentKind.XHtml,
            _ => TextSyndicationContentKind.Plaintext
        };

        var newDescription = originalFeed.Description.Type switch
        {
            "html" or "xhtml" =>
                $"<p>[This is a Podcast\u2009⏪\u2009Rewind feed. See link for details.]</p> {RewoundFeed.Description.Text}",
            _ =>
                $"[This is a Podcast\u2009⏪\u2009Rewind feed. See link for details.] {RewoundFeed.Description.Text}"
        };

        RewoundFeed.Description = new TextSyndicationContent(newDescription, newDescriptionKind);
    }

    private void LoadUpcomingFeed()
    {
        if (string.IsNullOrEmpty(feedRewindInfo.FeedUrl) || RewoundFeed is null) return;
        ScheduledFeed = RewoundFeed.Clone(false);
        ScheduledFeed.Items = AllRescheduledFeedItems
            .Where(item => item.PublishDate > DateTimeOffset.Now)
            .OrderBy(item => item.PublishDate);
    }

    public async Task<byte[]> GetRewoundFeedAsBytesAsync()
    {
        if (RewoundFeed is null) LoadRewoundFeed();
        if (RewoundFeed is null) return Array.Empty<byte>();

        var settings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            NewLineHandling = NewLineHandling.Entitize,
            NewLineOnAttributes = false,
            Indent = false,
            Async = true,
        };

        using var stream = new MemoryStream();
        await using var xmlWriter = XmlWriter.Create(stream, settings);

        var formatter = new Rss20FeedFormatter(RewoundFeed, true);
        formatter.WriteTo(xmlWriter);
        await xmlWriter.FlushAsync();

        return stream.ToArray();
    }
}
