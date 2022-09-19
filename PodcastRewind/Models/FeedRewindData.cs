using PodcastRewind.Models.Entities;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace PodcastRewind.Models;

public class FeedRewindData
{
    private readonly bool _loadScheduledFeed;
    private readonly string _feedPage;

    public FeedRewindData(FeedRewind feedRewind, string feedPage, bool loadScheduledFeed = false)
    {
        _loadScheduledFeed = loadScheduledFeed;
        _feedPage = feedPage;
        FeedRewind = feedRewind;
        LoadFeed();
    }

    private SyndicationFeed? OriginalFeed { get; set; }

    public bool OriginalFeedIsNull { get; private set; } = true;
    public string OriginalFeedTitle { get; private set; } = string.Empty;
    public FeedRewind FeedRewind { get; private set; }
    public SyndicationFeed? RewoundFeed { get; private set; }
    public SyndicationFeed? ScheduledFeed { get; private set; }

    private void LoadFeed()
    {
        if (string.IsNullOrEmpty(FeedRewind.FeedUrl)) return;
        using var xmlReader = XmlReader.Create(FeedRewind.FeedUrl);
        OriginalFeed = SyndicationFeed.Load(xmlReader);
        if (OriginalFeed is null) return;

        OriginalFeedIsNull = false;
        OriginalFeedTitle = OriginalFeed.Title.Text;

        var entries = OriginalFeed.Items.OrderBy(e => e.PublishDate).ToList();
        var feedItemsCount = entries.Count;
        var keyIndex = entries.FindIndex(e => e.Id == FeedRewind.KeyEntryId);
        var dateOfFirstEntry = FeedRewind.DateOfKeyEntry.AddDays(-FeedRewind.Interval * (keyIndex));

        for (var i = 0; i < feedItemsCount; i++)
        {
            var originalPublishDate = entries[i].PublishDate;
            entries[i].PublishDate = dateOfFirstEntry.AddDays(FeedRewind.Interval * i);
            entries[i].Summary = new TextSyndicationContent(string.Concat(
                $"[Originally published {originalPublishDate.ToString("MMMM d, yyyy")}.] ",
                entries[i].Summary.Text));
        }

        RewoundFeed = OriginalFeed.Clone(true);
        RewoundFeed.Title = new TextSyndicationContent($"⏪: {RewoundFeed.Title.Text}");

        var newDescription = string.Concat(
            "<p>This is a Podcast\u2009⏪\u2009Rewind feed. More information can be found at " +
            $"the <a href='{_feedPage}.'>Feed Page</a>.</p>",
            RewoundFeed.Description.Text);

        RewoundFeed.Description = new TextSyndicationContent(newDescription, TextSyndicationContentKind.Html);

        RewoundFeed.Items = entries.Where(e => e.PublishDate <= DateTimeOffset.Now)
            .OrderByDescending(e => e.PublishDate);

        if (_loadScheduledFeed)
        {
            ScheduledFeed = OriginalFeed.Clone(true);
            ScheduledFeed.Items = entries.Where(e => e.PublishDate > DateTimeOffset.Now)
                .OrderBy(e => e.PublishDate);
        }
    }

    public byte[] GetRewoundFeed()
    {
        if (RewoundFeed is null) return Array.Empty<byte>();

        var settings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            NewLineHandling = NewLineHandling.Entitize,
            NewLineOnAttributes = false,
            Indent = false
        };

        using var stream = new MemoryStream();
        using var xmlWriter = XmlWriter.Create(stream, settings);

        var rssFormatter = new Rss20FeedFormatter(RewoundFeed, false);
        rssFormatter.WriteTo(xmlWriter);
        xmlWriter.Flush();

        return stream.ToArray();
    }
}
