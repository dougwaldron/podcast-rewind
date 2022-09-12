using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PodcastRewind.Models;
using PodcastRewind.Services;
using System.ServiceModel.Syndication;
using System.Xml;

namespace PodcastRewind.Pages;

public class CreateModel : PageModel
{
    private readonly IFeedRewindRepository _repository;
    public CreateModel(IFeedRewindRepository repository) => _repository = repository;

    [BindProperty]
    public CreateFeedRewindDto CreateFeedRewind { get; set; } = null!;

    public string PodcastTitle { get; private set; } = string.Empty;
    public List<ViewPodcastEpisodeDto> PodcastEpisodes { get; private set; } = new();

    public IActionResult OnGet(string? feedUrl)
    {
        if (feedUrl is null) return NotFound();
        using var xmlReader = XmlReader.Create(feedUrl);
        var feed = SyndicationFeed.Load(xmlReader);
        if (feed is null) return NotFound();

        LoadData(feed);

        if (PodcastEpisodes.Count > 0)
        {
            CreateFeedRewind = new CreateFeedRewindDto
            {
                FeedUrl = feedUrl,
                KeyEntryId = PodcastEpisodes.First().Id,
            };
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            using var xmlReader = XmlReader.Create(CreateFeedRewind.FeedUrl);
            var feed = SyndicationFeed.Load(xmlReader);
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
    }
}
