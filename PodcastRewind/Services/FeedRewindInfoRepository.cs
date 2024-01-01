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

        await SaveFeedRewindInfoToFileAsync(id, feedRewind);
        return id;
    }

    public async Task UpdateAsync(EditFeedRewindInfoDto edit)
    {
        var original = await GetAsync(edit.Id)
                       ?? throw new ArgumentException($"Item {edit.Id} does not exist.", nameof(edit));

        var feedRewind = new FeedRewindInfo
        {
            Id = edit.Id,
            FeedUrl = edit.FeedUrl,
            KeyEntryId = edit.KeyEntryId,
            DateOfKeyEntry = edit.DateOfKeyEntry,
            Interval = edit.Interval,
            CreatedOn = original.CreatedOn,
        };

        await SaveFeedRewindInfoToFileAsync(edit.Id, feedRewind);
    }

    public async Task<FeedRewindInfo?> GetAsync(Guid id)
    {
        if (cache.TryGetValue(id, out FeedRewindInfo? info)) return info;
        info = await LoadFeedRewindInfoFromFileAsync(id);
        if (info != null) cache.Set(id, info, CacheEntryOptions);
        return info;
    }

    private async Task SaveFeedRewindInfoToFileAsync(Guid id, FeedRewindInfo feedRewind)
    {
        var filePath = Path.Combine(_dataFilesDirectory, string.Concat(id.ToString(), ".json"));
        Directory.CreateDirectory(_dataFilesDirectory);
        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, feedRewind);
        cache.Set(id, feedRewind, CacheEntryOptions);
    }

    private async Task<FeedRewindInfo?> LoadFeedRewindInfoFromFileAsync(Guid id)
    {
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
    }
}
