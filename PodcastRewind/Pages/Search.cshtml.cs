using iTunesPodcastFinder;
using iTunesPodcastFinder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace PodcastRewind.Pages;

public class SearchModel : PageModel
{
    [BindProperty]
    [Display(Name = "Search for:")]
    public string? Search { get; set; }

    public List<Podcast> SearchResults { get; private set; } = new();
    public bool ShowSearchResults { get; private set; }

    [BindProperty]
    [Display(Name = "Feed URL:")]
    public string? FeedUrl { get; set; }

    private readonly PodcastFinder _finder = new();

    public async Task<IActionResult> OnGetAsync(string? search)
    {
        if (search is null) return Page();
        return await GetAsyncInternal(search);
    }

    public async Task<IActionResult> GetAsyncInternal(string search)
    {
        // Default: retrieve 30 items using the US search
        try
        {
            SearchResults = (await _finder.SearchPodcastsAsync(search, 30))
                .Where(podcast => podcast.FeedUrl is not null).ToList();
            ShowSearchResults = true;
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("Search",
                "The iTunes podcast search service did not respond.");
        }

        return Page();
    }
}
