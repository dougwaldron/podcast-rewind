using iTunesPodcastFinder;
using iTunesPodcastFinder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace PodcastRewind.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    [Display(Name = "Search for:")]
    public string? Search { get; set; }

    public List<Podcast> SearchResults { get; private set; } = new();

    [BindProperty]
    [Display(Name = "Enter Feed URL")]
    public string? FeedUrl { get; set; }

    private readonly PodcastFinder _finder;
    public IndexModel() => _finder = new PodcastFinder();

    public async Task<IActionResult> OnGetAsync(string? search)
    {
        if (search is null) return Page();

        // Default: retrieve 30 items using the US search
        // TODO: Consider internationalizing?
        try
        {
            SearchResults = (await _finder.SearchPodcastsAsync(search, 30))
                .Where(p => p.FeedUrl is not null).ToList();
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("Search",
                "The iTunes poadcast search service did not respond.");
        }

        return Page();
    }
}
