using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PodcastRewind.Models;
using PodcastRewind.Services;
using System.ServiceModel.Syndication;

namespace PodcastRewind.Pages;

public class DetailsModel : PageModel
{
    private readonly IFeedRewindRepository _repository;
    private readonly ISyndicationFeedService _syndicationFeedService;

    public DetailsModel(
        IFeedRewindRepository repository,
        ISyndicationFeedService syndicationFeedService)
    {
        _repository = repository;
        _syndicationFeedService = syndicationFeedService;
    }

    public FeedRewindData FeedRewindData { get; private set; } = null!;
    public string RewindFeedUrl { get; private set; } = string.Empty;
    public string? OriginalPodcastLink { get; private set; }
    public string ApplePodcastSubscribeUrl { get; private set; } = string.Empty;
    public Guid RewindFeedId { get; private set; }
    public SyndicationFeed? RewoundFeed { get; private set; }
    public SyndicationFeed? ScheduledFeed { get; private set; }
    public string PodcastImageUrl { get; private set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id is null) return RedirectToPage("/Index");
        var feedRewindInfo = await _repository.GetAsync(id.Value);
        if (feedRewindInfo is null) return NotFound($"Feed ID '{id}' not found.");

        var originalFeed = await _syndicationFeedService.GetSyndicationFeedAsync(feedRewindInfo.FeedUrl);
        if (originalFeed is null) return NotFound();

        var feedPage = Url.PageLink("Details", values: new { id })!;
        FeedRewindData = new FeedRewindData(feedRewindInfo, originalFeed, feedPage);
        RewoundFeed = FeedRewindData.GetRewoundFeed();

        if (RewoundFeed is null) return NotFound($"Feed could not be loaded.");

        ScheduledFeed = FeedRewindData.GetScheduledFeed();
        RewindFeedId = id.Value;
        RewindFeedUrl = Url.ActionLink("GetFeed", "Feed", new { id })!;
        ApplePodcastSubscribeUrl = Url.ActionLink("GetFeed", "Feed", new { id }, "podcast")!;
        PodcastImageUrl = RewoundFeed.ImageUrl?.ToString() ?? "";
        OriginalPodcastLink = FeedRewindData.OriginalFeedLink?.AbsoluteUri;

        return Page();
    }
}
