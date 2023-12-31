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
    public Task<SyndicationFeed?> GetSyndicationFeedAsync(string url) =>
        !Uri.IsWellFormedUriString(url, UriKind.Absolute)
            ? Task.FromResult<SyndicationFeed?>(null)
            : cache.GetOrCreateAsync(url, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(20);
                return GetRemoteSyndicationFeedAsync(url);
            });

    private async Task<SyndicationFeed> GetRemoteSyndicationFeedAsync(string url)
    {
        var client = httpClientFactory.CreateClient("Polly");
        client.DefaultRequestHeaders.Add("user-agent", "PodcastRewind/1.0");
        await using var stream = await client.GetStreamAsync(url);
        using var xmlReader = XmlReader.Create(stream);
        return SyndicationFeed.Load(xmlReader);
    }
}
