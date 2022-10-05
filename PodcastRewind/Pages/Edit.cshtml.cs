using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PodcastRewind.Models;
using PodcastRewind.Models.DTOs;
using PodcastRewind.Services;
using System.ServiceModel.Syndication;

namespace PodcastRewind.Pages;

public class EditModel : PageModel
{
    private readonly IFeedRewindRepository _repository;
    public EditModel(IFeedRewindRepository repository) => _repository = repository;

    [BindProperty]
    public EditFeedRewindDto EditFeedRewind { get; set; } = null!;

    public string PodcastTitle { get; private set; } = string.Empty;
    public string PodcastImageUrl { get; private set; } = string.Empty;
    public List<ViewPodcastEpisodeDto> PodcastEpisodes { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id is null) return RedirectToPage("/Index");
        var feedRewindInfo = await _repository.GetAsync(id.Value);
        if (feedRewindInfo is null) return NotFound($"Feed ID '{id}' not found.");

        var feedRewindData = new FeedRewindData(feedRewindInfo);
        var rewoundFeed = await feedRewindData.GetRewoundFeedAsync();
        if (rewoundFeed is null) return NotFound($"Feed ID '{id}' not found.");

        var latestRewindEpisode = rewoundFeed.Items.FirstOrDefault();

        var feed = await feedRewindData.GetOriginalFeedAsync();
        if (feed is null) return NotFound();

        LoadData(feed);

        if (PodcastEpisodes.Count > 0)
        {
            EditFeedRewind = new EditFeedRewindDto
            {
                Id = feedRewindInfo.Id,
                FeedUrl = feedRewindInfo.FeedUrl,
                KeyEntryId = latestRewindEpisode?.Id ?? feedRewindInfo.KeyEntryId,
                DateOfKeyEntry = latestRewindEpisode?.PublishDate.Date ?? feedRewindInfo.DateOfKeyEntry,
                Interval = feedRewindInfo.Interval,
            };
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            var feed = await FeedRewindData.GetSyndicationFeedAsync(EditFeedRewind.FeedUrl);
            if (feed is null) return NotFound();

            LoadData(feed);
            return Page();
        }

        await _repository.UpdateAsync(EditFeedRewind);
        return RedirectToPage("Details", new { EditFeedRewind.Id });
    }

    private void LoadData(SyndicationFeed feed)
    {
        PodcastTitle = feed.Title.Text;
        PodcastEpisodes = feed.Items.Select(e => new ViewPodcastEpisodeDto(e))
            .OrderBy(e => e.PublishDate).ToList();
        PodcastImageUrl = feed.ImageUrl?.ToString() ?? "";
    }
}
