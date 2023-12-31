using System.ServiceModel.Syndication;
using System.Xml;
using PodcastRewind.Services;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace PodcastRewind.TestData;

public class TestSyndicationFeedService : ISyndicationFeedService
{
    public async Task<SyndicationFeed?> GetSyndicationFeedAsync(string url)
    {
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) return null;
        using var xmlReader = XmlReader.Create(new StringReader(Data.SamplePodcastFeed));
        return SyndicationFeed.Load(xmlReader);
    }
}
