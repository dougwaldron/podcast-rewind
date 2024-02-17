using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.Extensions.Caching.Memory;
using PodcastRewind.Services;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace PodcastRewind.TestData;

public class TestSyndicationFeedService(IMemoryCache cache) : ISyndicationFeedService
{
    private static readonly MemoryCacheEntryOptions CacheEntryOptions = new MemoryCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromMinutes(3));

    public async Task<SyndicationFeed?> GetSyndicationFeedAsync(string url)
    {
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) return null;
        if (cache.TryGetValue(url, out SyndicationFeed? feed)) return feed;
        feed = await GetRemoteSyndicationFeedAsync();
        if (feed != null) cache.Set(url, feed, CacheEntryOptions);
        return feed;
    }

    private static async Task<SyndicationFeed?> GetRemoteSyndicationFeedAsync()
    {
        using var xmlReader = XmlReader.Create(new StringReader(Data.SamplePodcastFeed));
        return SyndicationFeed.Load(xmlReader);
    }
}
