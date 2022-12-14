using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PodcastRewind.Models.DTOs;
using PodcastRewind.Services;
using System.ServiceModel.Syndication;

namespace PodcastRewind.Pages;

public class SetupModel : PageModel
{
    private readonly IFeedRewindRepository _repository;
    private readonly ISyndicationFeedService _syndicationFeedService;

    public SetupModel(IFeedRewindRepository repository, ISyndicationFeedService syndicationFeedService)
    {
        _repository = repository;
        _syndicationFeedService = syndicationFeedService;
    }

    [BindProperty]
    public CreateFeedRewindDto CreateFeedRewind { get; set; } = null!;

    public string PodcastTitle { get; private set; } = string.Empty;
    public string PodcastImageUrl { get; private set; } = string.Empty;
    public List<ViewPodcastEpisodeDto> PodcastEpisodes { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(string? feedUrl, int interval = 7)
    {
        if (feedUrl is null) return BadRequest("No feed URL entered.");
        if (!Uri.IsWellFormedUriString(feedUrl, UriKind.Absolute)) return BadRequest("The feed URL is invalid.");

        var feed = await _syndicationFeedService.GetSyndicationFeedAsync(feedUrl);
        if (feed is null) return NotFound();

        LoadData(feed);

        if (PodcastEpisodes.Count > 0)
        {
            CreateFeedRewind = new CreateFeedRewindDto
            {
                FeedUrl = feedUrl,
                KeyEntryId = PodcastEpisodes.First().Id,
                Interval = interval,
            };
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            var feed = await _syndicationFeedService.GetSyndicationFeedAsync(CreateFeedRewind.FeedUrl);
            if (feed is null) return NotFound();

            LoadData(feed);
            return Page();
        }

        var id = await _repository.SaveAsync(CreateFeedRewind);
        return RedirectToPage("Details", new { id });
    }

    private void LoadData(SyndicationFeed feed)
    {
        PodcastTitle = feed.Title.Text;
        PodcastEpisodes = feed.Items.Select(e => new ViewPodcastEpisodeDto(e))
            .OrderBy(e => e.PublishDate).ToList();
        PodcastImageUrl = feed.ImageUrl?.ToString() ?? "";
    }
}
