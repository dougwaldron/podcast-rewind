using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.Extensions.Caching.Memory;

namespace PodcastRewind.Services;

public interface ISyndicationFeedService
{
    Task<SyndicationFeed?> GetSyndicationFeedAsync(string url);
}

public class SyndicationFeedService(IHttpClientFactory httpClientFactory, IMemoryCache cache)
    : ISyndicationFeedService
{
    private static readonly MemoryCacheEntryOptions CacheEntryOptions = new MemoryCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromHours(20));

    public async Task<SyndicationFeed?> GetSyndicationFeedAsync(string url)
    {
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) return null;
        if (cache.TryGetValue(url, out SyndicationFeed? feed)) return feed;
        feed = await GetRemoteSyndicationFeedAsync(url);
        if (feed != null) cache.Set(url, feed, CacheEntryOptions);
        return feed;
    }

    private async Task<SyndicationFeed?> GetRemoteSyndicationFeedAsync(string url)
    {
        var client = httpClientFactory.CreateClient(nameof(Polly));
        client.DefaultRequestHeaders.Add("user-agent", "PodcastRewind/1.0");
        try
        {
            await using var stream = await client.GetStreamAsync(url);
            using var xmlReader = XmlReader.Create(stream);
            return SyndicationFeed.Load(xmlReader);
        }
        catch (HttpRequestException e)
        {
            SentrySdk.CaptureException(e);
            return null;
        }
    }
}
