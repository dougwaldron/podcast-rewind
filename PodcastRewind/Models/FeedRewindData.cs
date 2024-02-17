using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using PodcastRewind.Models.Entities;

namespace PodcastRewind.Models;

public class FeedRewindData(FeedRewindInfo feedRewindInfo, SyndicationFeed originalFeed, string? feedPageLink = null)
{
    public const string FeedMimeType = "text/xml; charset=utf-8";
    private const string ContentNamespace = "http://purl.org/rss/1.0/modules/content/";
    private const string ContentEncodedName = "encoded";
    private const string ItunesNamespace = "http://www.itunes.com/dtds/podcast-1.0.dtd";
    private const string ItunesSummaryName = "summary";

    private readonly Uri? _feedPageUri = feedPageLink is null ? null : new Uri(feedPageLink);

    public Uri? OriginalFeedLink { get; } =
        originalFeed.Links.FirstOrDefault(link => link.RelationshipType == "alternate")?.Uri;

    public string FeedTitle { get; } = originalFeed.Title.Text;
    private SyndicationFeed? RewoundFeed { get; set; }
    private DateTimeOffset? MostRecentRewoundFeedEntryDate { get; set; }
    private List<SyndicationItem> AllRescheduledFeedItems { get; set; } = [];

    public SyndicationFeed? GetRewoundFeed()
    {
        if (RewoundFeed is null) BuildRewoundFeed();
        return RewoundFeed;
    }

    public IEnumerable<SyndicationItem> GetUpcomingItems() =>
        AllRescheduledFeedItems
            .Where(item => item.PublishDate > DateTimeOffset.Now)
            .OrderBy(item => item.PublishDate);

    public SyndicationFeed GetOriginalFeed() => originalFeed;
    public FeedRewindInfo GetFeedRewindInfo() => feedRewindInfo;

    public DateTimeOffset GetLastModifiedDate()
    {
        if (MostRecentRewoundFeedEntryDate is null) BuildRewoundFeed();
        return MostRecentRewoundFeedEntryDate ?? DateTimeOffset.Now;
    }

    public string GetETag() =>
        $"\"{Convert.ToBase64String(Encoding.UTF8.GetBytes(GetLastModifiedDate().ToString()))}\"";

    private void BuildRewoundFeed()
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

            // Change pub date.
            var originalPubDate = feedItem.PublishDate;
            var pubDateDescription = $"<p>[⏪\u2009Originally published {originalPubDate:MMMM d, yyyy}.]</p>";
            feedItem.PublishDate = new DateTimeOffset(dateOfFirstEntry, originalPubDate.Offset)
                .Add(originalPubDate.TimeOfDay).AddDays(feedRewindInfo.Interval * i);

            // Update summary and content with original pub date.
            feedItem.Summary = new TextSyndicationContent($"{pubDateDescription} {feedItem.Summary?.Text}");
            UpdateExtensionContent(feedItem, pubDateDescription, ContentEncodedName, ContentNamespace);
            UpdateExtensionContent(feedItem, pubDateDescription, ItunesSummaryName, ItunesNamespace);
        }

        RewoundFeed.Items = AllRescheduledFeedItems.Where(item => item.PublishDate <= DateTimeOffset.Now)
            .OrderByDescending(item => item.PublishDate);
        if (AllRescheduledFeedItems.Exists(item => item.PublishDate <= DateTimeOffset.Now))
            MostRecentRewoundFeedEntryDate = RewoundFeed.Items.First().PublishDate;

        RewoundFeed.Title = new TextSyndicationContent($"⏪ {RewoundFeed.Title.Text}");
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

    private static void UpdateExtensionContent(SyndicationItem feedItem, string pubDateDescription,
        string outerName, string outerNamespace)
    {
        if (!feedItem.ElementExtensions.Any(extension =>
                extension.OuterName == outerName && extension.OuterNamespace == outerNamespace))
            return;
        var content = feedItem.ElementExtensions.ReadElementExtensions<string>(outerName, outerNamespace)[0];
        foreach (var extension in feedItem.ElementExtensions.Where(extension =>
                     extension.OuterName == outerName && extension.OuterNamespace == outerNamespace).ToList())
            feedItem.ElementExtensions.Remove(extension);
        feedItem.ElementExtensions.Add(outerName, outerNamespace, $"{pubDateDescription} {content}");
    }

    public async Task<byte[]> GetRewoundFeedAsBytesAsync()
    {
        if (RewoundFeed is null) BuildRewoundFeed();
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
