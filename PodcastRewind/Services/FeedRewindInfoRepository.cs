using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using PodcastRewind.Models.Dto;
using PodcastRewind.Models.Entities;

namespace PodcastRewind.Services;

public interface IFeedRewindInfoRepository
{
    Task<Guid> SaveAsync(CreateFeedRewindInfoDto create);
    Task UpdateAsync(EditFeedRewindInfoDto edit);
    Task<FeedRewindInfo?> GetAsync(Guid id);
}

public class FeedRewindInfoRepository(IConfiguration config, IMemoryCache cache)
    : IFeedRewindInfoRepository
{
    private static readonly MemoryCacheEntryOptions CacheEntryOptions = new MemoryCacheEntryOptions()
        .SetSlidingExpiration(TimeSpan.FromHours(3))
        .SetAbsoluteExpiration(TimeSpan.FromDays(1));

    private readonly string _dataFilesDirectory = config.GetValue<string>("DataFilesDirectory")!;

    public async Task<Guid> SaveAsync(CreateFeedRewindInfoDto create)
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

        var filePath = Path.Combine(_dataFilesDirectory, string.Concat(id.ToString(), ".json"));
        Directory.CreateDirectory(_dataFilesDirectory);
        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, feedRewind);

        cache.Set(id, feedRewind, CacheEntryOptions);
        return id;
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

        var filePath = Path.Combine(_dataFilesDirectory, string.Concat(feedRewind.Id.ToString(), ".json"));
        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, feedRewind);
        cache.Set(edit.Id, feedRewind, CacheEntryOptions);
    }

    public Task<FeedRewindInfo?> GetAsync(Guid id) =>
        cache.GetOrCreateAsync(id, async entry =>
        {
            entry.SetOptions(CacheEntryOptions);
            var filePath = Path.Combine(_dataFilesDirectory, string.Concat(id.ToString(), ".json"));
            try
            {
                await using var stream = File.OpenRead(filePath);
                return await JsonSerializer.DeserializeAsync<FeedRewindInfo>(stream);
            }
            catch (Exception e) when (e is FileNotFoundException or DirectoryNotFoundException)
            {
                return null;
            }
        });
}
