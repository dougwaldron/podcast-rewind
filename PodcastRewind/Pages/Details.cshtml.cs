using System.ServiceModel.Syndication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PodcastRewind.Models;
using PodcastRewind.Services;

namespace PodcastRewind.Pages;

public class DetailsModel(IFeedRewindDataService feedService)
    : PageModel
{
    public FeedRewindData? FeedRewindData { get; private set; }
    public string RewindFeedUrl { get; private set; } = string.Empty;
    public string? OriginalPodcastLink { get; private set; }
    public string ApplePodcastSubscribeUrl { get; private set; } = string.Empty;
    public Guid RewindFeedId { get; private set; }
    public SyndicationFeed? RewoundFeed { get; private set; }
    public List<SyndicationItem> ScheduledItems { get; } = [];
    public string PodcastImageUrl { get; private set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id is null) return RedirectToPage("/Index");

        var feedPageLink = Url.PageLink("Details", values: new { id })!;
        FeedRewindData = await feedService.GetFeedRewindDataAsync(id.Value, feedPageLink);
        if (FeedRewindData == null) return NotFound($"Feed ID '{id}' not found.");

        RewoundFeed = FeedRewindData.GetRewoundFeed();
        if (RewoundFeed is null) return NotFound($"Feed could not be loaded.");

        ScheduledItems.AddRange(FeedRewindData.GetUpcomingItems());
        RewindFeedId = id.Value;
        RewindFeedUrl = Url.ActionLink("Get", "Feed", new { id })!;
        ApplePodcastSubscribeUrl = Url.ActionLink("Get", "Feed", new { id }, "podcast")!;
        PodcastImageUrl = RewoundFeed.ImageUrl?.ToString() ?? "";
        OriginalPodcastLink = FeedRewindData.OriginalFeedLink?.AbsoluteUri;

        return Page();
    }
}
