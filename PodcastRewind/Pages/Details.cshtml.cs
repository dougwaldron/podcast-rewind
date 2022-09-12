using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PodcastRewind.Models;
using PodcastRewind.Services;

namespace PodcastRewind.Pages;

public class DetailsModel : PageModel
{
    public FeedRewindData FeedRewindData { get; set; } = null!;
    public string RewindFeedUrl { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(
        [FromServices] IFeedRewindRepository repository,
        Guid? id)
    {
        if (id is null) return RedirectToPage("/Index");
        var feed = await repository.GetAsync(id.Value);
        if (feed is null) return NotFound($"Feed ID '{id}' not found.");

        RewindFeedUrl = Url.ActionLink("GetFeed", "Feed", new { id })!;
        var feedPage = Url.PageLink("Details", values: new { id })!;

        FeedRewindData = new FeedRewindData(feed, feedPage, true);
        if (FeedRewindData.OriginalFeedIsNull) return NotFound($"Feed could not be loaded.");

        return Page();
    }
}
