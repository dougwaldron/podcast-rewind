using PodcastRewind.Models.Dto;
using PodcastRewind.Models.Entities;
using PodcastRewind.Services;

namespace PodcastRewind.TestData;

public class TestFeedRewindInfoRepository : IFeedRewindInfoRepository
{
    public Task<Guid> SaveAsync(CreateFeedRewindInfoDto create)
    {
        var id = Guid.NewGuid();

        var info = new FeedRewindInfo
        {
            Id = id,
            FeedUrl = create.FeedUrl,
            KeyEntryId = create.KeyEntryId,
            DateOfKeyEntry = create.DateOfKeyEntry,
            Interval = create.Interval,
        };

        Data.FeedRewindInfoData.Add(info);

        return Task.FromResult(id);
    }

    public async Task UpdateAsync(EditFeedRewindInfoDto edit)
    {
        var original = await GetAsync(edit.Id);
        if (original is null) throw new ArgumentException($"Item {edit.Id} does not exist.", nameof(edit));

        var info = new FeedRewindInfo
        {
            Id = edit.Id,
            FeedUrl = edit.FeedUrl,
            KeyEntryId = edit.KeyEntryId,
            DateOfKeyEntry = edit.DateOfKeyEntry,
            Interval = edit.Interval,
            CreatedOn = original.CreatedOn,
        };

        Data.FeedRewindInfoData.Remove(original);
        Data.FeedRewindInfoData.Add(info);
    }

    public Task<FeedRewindInfo?> GetAsync(Guid id) =>
        Task.FromResult(Data.FeedRewindInfoData.SingleOrDefault(info => info.Id == id));
}
