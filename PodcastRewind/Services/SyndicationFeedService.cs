using System.ServiceModel.Syndication;
using System.Xml;

namespace PodcastRewind.Services;

public interface ISyndicationFeedService
{
    Task<SyndicationFeed?> GetSyndicationFeedAsync(string url);
}

public class SyndicationFeedService : ISyndicationFeedService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SyndicationFeedService(IHttpClientFactory httpClientFactory) =>
        _httpClientFactory = httpClientFactory;

    public async Task<SyndicationFeed?> GetSyndicationFeedAsync(string url)
    {
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) return null;
        var client = _httpClientFactory.CreateClient("Polly");
        client.DefaultRequestHeaders.Add("user-agent", "PodcastRewind/1.0");
        await using var stream = await client.GetStreamAsync(url);
        using var xmlReader = XmlReader.Create(stream);
        return SyndicationFeed.Load(xmlReader);
    }
}
