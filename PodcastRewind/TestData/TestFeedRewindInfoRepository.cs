using Microsoft.Extensions.Caching.Memory;
using PodcastRewind.Models.Dto;
using PodcastRewind.Models.Entities;
using PodcastRewind.Services;

namespace PodcastRewind.TestData;

public class TestFeedRewindInfoRepository(IMemoryCache cache) : IFeedRewindInfoRepository
{
    private static readonly MemoryCacheEntryOptions CacheEntryOptions = new MemoryCacheEntryOptions()
        .SetSlidingExpiration(TimeSpan.FromHours(3))
        .SetAbsoluteExpiration(TimeSpan.FromDays(1));

    public Task<Guid> SaveAsync(CreateFeedRewindInfoDto create)
    {
        var id = Guid.NewGuid();

        var feedRewind = new FeedRewindInfo
        {
            Id = id,
            FeedUrl = create.FeedUrl,
            KeyEntryId = create.KeyEntryId,
            DateOfKeyEntry = create.DateOfKeyEntry,
            Interval = create.Interval,
        };

        Data.FeedRewindInfoData.Add(feedRewind);
        cache.Set(id, feedRewind, CacheEntryOptions);

        return Task.FromResult(id);
    }

    public async Task UpdateAsync(EditFeedRewindInfoDto edit)
    {
        var original = await GetAsync(edit.Id);
        if (original is null) throw new ArgumentException($"Item {edit.Id} does not exist.", nameof(edit));

        var feedRewind = new FeedRewindInfo
        {
            Id = edit.Id,
            FeedUrl = edit.FeedUrl,
            KeyEntryId = edit.KeyEntryId,
            DateOfKeyEntry = edit.DateOfKeyEntry,
            Interval = edit.Interval,
            CreatedOn = original.CreatedOn,
        };

        Data.FeedRewindInfoData.Remove(original);
        Data.FeedRewindInfoData.Add(feedRewind);
        cache.Set(edit.Id, feedRewind, CacheEntryOptions);
    }

    public Task<FeedRewindInfo?> GetAsync(Guid id)
    {
        if (cache.TryGetValue(id, out FeedRewindInfo? feedRewind)) return Task.FromResult(feedRewind);
        feedRewind = Data.FeedRewindInfoData.SingleOrDefault(info => info.Id == id);
        if (feedRewind != null) cache.Set(id, feedRewind, CacheEntryOptions);
        return Task.FromResult(feedRewind);
    }

    public async Task UpdateLastAccessedAsync(Guid id)
    {
        var feedRewind = await GetAsync(id);
        if (feedRewind == null) return;

        var updatedFeedRewind = feedRewind with { LastAccessedOn = DateTime.Now };
        Data.FeedRewindInfoData.Remove(feedRewind);
        Data.FeedRewindInfoData.Add(updatedFeedRewind);
        cache.Set(id, updatedFeedRewind, CacheEntryOptions);
    }
}
