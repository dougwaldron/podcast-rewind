using PodcastRewind.Models.Entities;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace PodcastRewind.Models;

public class FeedRewindData
{
    private readonly FeedRewindInfo _feedRewindInfo;
    private readonly Uri? _feedPageUri;

    public FeedRewindData(FeedRewindInfo feedRewindInfo, string? feedPageLink)
    {
        _feedRewindInfo = feedRewindInfo;
        _feedPageUri = feedPageLink is null ? null : new Uri(feedPageLink);
    }

    public string FeedTitle { get; private set; } = string.Empty;

    private SyndicationFeed? OriginalFeed { get; set; }
    private SyndicationFeed? RewoundFeed { get; set; }
    private SyndicationFeed? ScheduledFeed { get; set; }
    private List<SyndicationItem> RewoundEntries { get; set; } = new();

    public async Task<SyndicationFeed?> GetRewoundFeedAsync()
    {
        if (RewoundFeed is null) await LoadRewoundFeedAsync();
        return RewoundFeed;
    }

    public async Task<SyndicationFeed?> GetScheduledFeedAsync()
    {
        if (ScheduledFeed is null) await LoadScheduledFeedAsync();
        return ScheduledFeed;
    }

    private async Task LoadRewoundFeedAsync()
    {
        if (string.IsNullOrEmpty(_feedRewindInfo.FeedUrl)) return;
        if (OriginalFeed is null && !await LoadOriginalFeedAsync()) return;

        RewoundEntries = OriginalFeed!.Items.OrderBy(e => e.PublishDate).ToList();
        var feedItemsCount = RewoundEntries.Count;
        var keyIndex = RewoundEntries.FindIndex(e => e.Id == _feedRewindInfo.KeyEntryId);
        var dateOfFirstEntry = _feedRewindInfo.DateOfKeyEntry.AddDays(-_feedRewindInfo.Interval * (keyIndex));

        for (var i = 0; i < feedItemsCount; i++)
        {
            var pubDate = RewoundEntries[i].PublishDate;
            var newPubDate = new DateTimeOffset(dateOfFirstEntry, pubDate.Offset)
                .Add(pubDate.TimeOfDay)
                .AddDays(_feedRewindInfo.Interval * i);

            RewoundEntries[i].PublishDate = newPubDate;
            RewoundEntries[i].Summary = new TextSyndicationContent(
                $"[⏪\u2009Originally published {pubDate:MMMM d, yyyy}.] {RewoundEntries[i].Summary.Text}");
        }

        RewoundFeed = OriginalFeed.Clone(true);
        RewoundFeed.Title = new TextSyndicationContent($"⏪: {RewoundFeed.Title.Text}");
        if (_feedPageUri is not null)
            RewoundFeed.Links.Insert(0, SyndicationLink.CreateAlternateLink(_feedPageUri, "text/html"));

        var descriptionType = OriginalFeed.Description.Type switch
        {
            "html" => TextSyndicationContentKind.Html,
            "xhtml" => TextSyndicationContentKind.XHtml,
            _ => TextSyndicationContentKind.Plaintext,
        };

        var newDescription = OriginalFeed.Description.Type switch
        {
            "html" or "xhtml" => string.Concat(
                "<p>[This is a Podcast\u2009⏪\u2009Rewind feed. See link for details.] </p>",
                RewoundFeed.Description.Text),
            _ => string.Concat(
                "[This is a Podcast\u2009⏪\u2009Rewind feed. See link for details.] ",
                RewoundFeed.Description.Text),
        };

        RewoundFeed.Description = new TextSyndicationContent(newDescription, descriptionType);
        RewoundFeed.Items = RewoundEntries.Where(e => e.PublishDate <= DateTimeOffset.Now)
            .OrderByDescending(e => e.PublishDate);
    }

    private async Task LoadScheduledFeedAsync()
    {
        if (string.IsNullOrEmpty(_feedRewindInfo.FeedUrl)) return;
        if (OriginalFeed is null && !await LoadOriginalFeedAsync()) return;
        ScheduledFeed = OriginalFeed!.Clone(true);
        ScheduledFeed.Items = RewoundEntries.Where(e => e.PublishDate > DateTimeOffset.Now)
            .OrderBy(e => e.PublishDate);
    }

    [MemberNotNullWhen(true, nameof(OriginalFeed))]
    private async Task<bool> LoadOriginalFeedAsync()
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("user-agent", "PodcastRewind/1.0");

        await using var stream = await client.GetStreamAsync(_feedRewindInfo.FeedUrl);
        using var xmlReader = XmlReader.Create(stream);

        OriginalFeed = SyndicationFeed.Load(xmlReader);
        if (OriginalFeed is null) return false;
        FeedTitle = OriginalFeed.Title.Text;

        return true;
    }

    public async Task<byte[]> GetRewoundFeedAsBytesAsync()
    {
        if (RewoundFeed is null) await LoadRewoundFeedAsync();
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

        var formatter = new Rss20FeedFormatter(RewoundFeed, false);
        formatter.WriteTo(xmlWriter);
        await xmlWriter.FlushAsync();

        return stream.ToArray();
    }
}
