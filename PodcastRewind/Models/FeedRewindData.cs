using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using PodcastRewind.Models.Entities;

namespace PodcastRewind.Models;

public class FeedRewindData
{
    public const string FeedMimeType = "text/xml; charset=utf-8";

    private readonly FeedRewindInfo _feedRewindInfo;
    private readonly SyndicationFeed _originalFeed;
    private readonly Uri? _feedPageUri;

    public string FeedTitle { get; private set; }
    public Uri? OriginalFeedLink { get; private set; }

    public FeedRewindData(
        FeedRewindInfo feedRewindInfo,
        SyndicationFeed originalFeed,
        string? feedPageLink = null)
    {
        _feedRewindInfo = feedRewindInfo;
        _originalFeed = originalFeed;
        _feedPageUri = feedPageLink is null ? null : new Uri(feedPageLink);

        FeedTitle = originalFeed.Title.Text;
        OriginalFeedLink = originalFeed.Links
            .FirstOrDefault(link => link.RelationshipType == "alternate")?.Uri;
    }

    private SyndicationFeed? RewoundFeed { get; set; }
    private DateTimeOffset? MostRecentRewoundFeedEntryDate { get; set; }
    private SyndicationFeed? ScheduledFeed { get; set; }
    private List<SyndicationItem> RewoundEntries { get; set; } = new();

    public SyndicationFeed? GetRewoundFeed()
    {
        if (RewoundFeed is null) LoadRewoundFeed();
        return RewoundFeed;
    }

    public DateTimeOffset GetLastModifiedDate()
    {
        if (MostRecentRewoundFeedEntryDate is null) LoadRewoundFeed();
        return MostRecentRewoundFeedEntryDate ?? DateTimeOffset.Now;
    }

    public SyndicationFeed? GetScheduledFeed()
    {
        if (ScheduledFeed is null) LoadScheduledFeed();
        return ScheduledFeed;
    }

    private void LoadRewoundFeed()
    {
        if (string.IsNullOrEmpty(_feedRewindInfo.FeedUrl)) return;

        RewoundEntries = _originalFeed.Items.OrderBy(item => item.PublishDate).ToList();
        var feedItemsCount = RewoundEntries.Count;
        var keyIndex = RewoundEntries.FindIndex(item => item.Id == _feedRewindInfo.KeyEntryId);
        var dateOfFirstEntry = _feedRewindInfo.DateOfKeyEntry.AddDays(-_feedRewindInfo.Interval * (keyIndex));

        for (var i = 0; i < feedItemsCount; i++)
        {
            var pubDate = RewoundEntries[i].PublishDate;
            var newPubDate = new DateTimeOffset(dateOfFirstEntry, pubDate.Offset)
                .Add(pubDate.TimeOfDay)
                .AddDays(_feedRewindInfo.Interval * i);

            RewoundEntries[i].PublishDate = newPubDate;
            RewoundEntries[i].Summary = new TextSyndicationContent(
                $"[⏪\u2009Originally published {pubDate:MMMM d, yyyy}.] {RewoundEntries[i].Summary?.Text}");
        }

        RewoundFeed = _originalFeed.Clone(true);
        RewoundFeed.Title = new TextSyndicationContent($"⏪: {RewoundFeed.Title.Text}");
        if (_feedPageUri is not null)
            RewoundFeed.Links.Insert(0, SyndicationLink.CreateAlternateLink(_feedPageUri, "text/html"));

        var descriptionType = _originalFeed.Description.Type switch
        {
            "html" => TextSyndicationContentKind.Html,
            "xhtml" => TextSyndicationContentKind.XHtml,
            _ => TextSyndicationContentKind.Plaintext,
        };

        var newDescription = _originalFeed.Description.Type switch
        {
            "html" or "xhtml" => string.Concat(
                "<p>[This is a Podcast\u2009⏪\u2009Rewind feed. See link for details.] </p>",
                RewoundFeed.Description.Text),
            _ => string.Concat(
                "[This is a Podcast\u2009⏪\u2009Rewind feed. See link for details.] ",
                RewoundFeed.Description.Text),
        };

        RewoundFeed.Description = new TextSyndicationContent(newDescription, descriptionType);
        RewoundFeed.Items = RewoundEntries.Where(item => item.PublishDate <= DateTimeOffset.Now)
            .OrderByDescending(item => item.PublishDate);
        MostRecentRewoundFeedEntryDate = RewoundFeed.Items.First().PublishDate;
    }

    private void LoadScheduledFeed()
    {
        if (string.IsNullOrEmpty(_feedRewindInfo.FeedUrl)) return;
        ScheduledFeed = _originalFeed.Clone(true);
        ScheduledFeed.Items = RewoundEntries.Where(item => item.PublishDate > DateTimeOffset.Now)
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
