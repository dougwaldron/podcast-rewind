using PodcastRewind.Models;

namespace PodcastRewind.Services;

public interface IFeedRewindDataService
{
    Task<FeedRewindData?> GetFeedRewindDataAsync(Guid id, string? feedPageLink = null);
}

public class FeedRewindDataService(IFeedRewindInfoRepository repository, ISyndicationFeedService feedService)
    : IFeedRewindDataService
{
    public async Task<FeedRewindData?> GetFeedRewindDataAsync(Guid id, string? feedPageLink = null)
    {
        var feedRewindInfo = await repository.GetAsync(id);
        if (feedRewindInfo is null) return null;
        var originalFeed = await feedService.GetSyndicationFeedAsync(feedRewindInfo.FeedUrl);
        return originalFeed is null ? null : new FeedRewindData(feedRewindInfo, originalFeed, feedPageLink);
    }
}
