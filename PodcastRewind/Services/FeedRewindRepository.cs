using PodcastRewind.Models.DTOs;
using PodcastRewind.Models.Entities;
using System.Text.Json;

namespace PodcastRewind.Services;

public interface IFeedRewindRepository
{
    Task<Guid> SaveAsync(CreateFeedRewindDto create);
    Task UpdateAsync(EditFeedRewindDto edit);
    Task<FeedRewindInfo?> GetAsync(Guid id);
}

public class FeedRewindRepository : IFeedRewindRepository
{
    private static string DataFilesDirectory => "./_DataFiles";

    public async Task<Guid> SaveAsync(CreateFeedRewindDto create)
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

        var filePath = Path.Combine(DataFilesDirectory, string.Concat(id.ToString(), ".json"));
        Directory.CreateDirectory(DataFilesDirectory);
        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, feedRewind);
        await stream.DisposeAsync();

        return id;
    }

    public async Task UpdateAsync(EditFeedRewindDto edit)
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

        var filePath = Path.Combine(DataFilesDirectory, string.Concat(feedRewind.Id.ToString(), ".json"));
        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, feedRewind);
        await stream.DisposeAsync();
    }

    public async Task<FeedRewindInfo?> GetAsync(Guid id)
    {
        var filePath = Path.Combine(DataFilesDirectory, string.Concat(id.ToString(), ".json"));

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
