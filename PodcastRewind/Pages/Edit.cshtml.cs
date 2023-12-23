using System.ServiceModel.Syndication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PodcastRewind.Models.Dto;
using PodcastRewind.Services;

namespace PodcastRewind.Pages;

public class EditModel(IFeedRewindDataService feedService, IFeedRewindInfoRepository repository)
    : PageModel
{
    [BindProperty] public EditFeedRewindInfoDto EditFeedRewindInfo { get; set; } = null!;

    public string PodcastTitle { get; private set; } = string.Empty;
    public string PodcastImageUrl { get; private set; } = string.Empty;
    public List<ViewPodcastEpisodeDto> PodcastEpisodes { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id is null) return RedirectToPage("/Index");

        var feedRewindData = await feedService.GetFeedRewindDataAsync(id.Value);
        if (feedRewindData == null) return NotFound($"Feed ID '{id}' not found.");

        var rewoundFeed = feedRewindData.GetRewoundFeed();
        if (rewoundFeed is null) return NotFound($"Feed ID '{id}' not found.");

        var feedRewindInfo = feedRewindData.GetFeedRewindInfo();
        var latestRewindEpisode = rewoundFeed.Items.FirstOrDefault();

        LoadData(feedRewindData.GetOriginalFeed());

        if (PodcastEpisodes.Count > 0)
        {
            EditFeedRewindInfo = new EditFeedRewindInfoDto
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
            var feedRewindData = await feedService.GetFeedRewindDataAsync(EditFeedRewindInfo.Id);
            if (feedRewindData is null) return NotFound();

            LoadData(feedRewindData.GetOriginalFeed());
            return Page();
        }

        await repository.UpdateAsync(EditFeedRewindInfo);
        return RedirectToPage("Details", new { EditFeedRewindInfo.Id });
    }

    private void LoadData(SyndicationFeed feed)
    {
        PodcastTitle = feed.Title.Text;
        PodcastEpisodes = feed.Items.Select(item => new ViewPodcastEpisodeDto(item))
            .OrderBy(dto => dto.PublishDate).ToList();
        PodcastImageUrl = feed.ImageUrl?.ToString() ?? "";
    }
}
