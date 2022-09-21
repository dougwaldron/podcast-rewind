using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PodcastRewind.Models;
using PodcastRewind.Services;
using System.ServiceModel.Syndication;

namespace PodcastRewind.Pages;

public class DetailsModel : PageModel
{
    public FeedRewindData FeedRewindData { get; private set; } = null!;
    public string RewindFeedUrl { get; private set; } = string.Empty;
    public Guid RewindFeedId { get; private set; }
    public SyndicationFeed? RewoundFeed { get; private set; }
    public SyndicationFeed? ScheduledFeed { get; private set; }

    public async Task<IActionResult> OnGetAsync(
        [FromServices] IFeedRewindRepository repository,
        Guid? id)
    {
        if (id is null) return RedirectToPage("/Index");
        var feed = await repository.GetAsync(id.Value);
        if (feed is null) return NotFound($"Feed ID '{id}' not found.");

        var feedPage = Url.PageLink("Details", values: new { id })!;
        FeedRewindData = new FeedRewindData(feed, feedPage);
        RewoundFeed = await FeedRewindData.GetRewoundFeedAsync();

        if (RewoundFeed is null) return NotFound($"Feed could not be loaded.");

        ScheduledFeed = await FeedRewindData.GetScheduledFeedAsync();
        RewindFeedId = id.Value;
        RewindFeedUrl = Url.ActionLink("GetFeed", "Feed", new { id })!;
        
        return Page();
    }
}
