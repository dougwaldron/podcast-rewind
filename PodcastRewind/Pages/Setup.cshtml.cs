﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PodcastRewind.Services;
using System.ServiceModel.Syndication;
using PodcastRewind.Models.Dto;

namespace PodcastRewind.Pages;

public class SetupModel(IFeedRewindRepository repository, ISyndicationFeedService syndicationFeedService)
    : PageModel
{
    [BindProperty]
    public CreateFeedRewindDto CreateFeedRewind { get; set; } = null!;

    public string PodcastTitle { get; private set; } = string.Empty;
    public string PodcastImageUrl { get; private set; } = string.Empty;
    public List<ViewPodcastEpisodeDto> PodcastEpisodes { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync(string? feedUrl, int interval = 7)
    {
        if (feedUrl is null) return BadRequest("No feed URL entered.");
        if (!Uri.IsWellFormedUriString(feedUrl, UriKind.Absolute)) return BadRequest("The feed URL is invalid.");

        var feed = await syndicationFeedService.GetSyndicationFeedAsync(feedUrl);
        if (feed is null) return NotFound();

        LoadData(feed);

        if (PodcastEpisodes.Count > 0)
        {
            CreateFeedRewind = new CreateFeedRewindDto
            {
                FeedUrl = feedUrl,
                KeyEntryId = PodcastEpisodes[0].Id,
                Interval = interval,
            };
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            var feed = await syndicationFeedService.GetSyndicationFeedAsync(CreateFeedRewind.FeedUrl);
            if (feed is null) return NotFound();

            LoadData(feed);
            return Page();
        }

        var id = await repository.SaveAsync(CreateFeedRewind);
        return RedirectToPage("Details", new { id });
    }

    private void LoadData(SyndicationFeed feed)
    {
        PodcastTitle = feed.Title.Text;
        PodcastEpisodes = feed.Items.Select(item => new ViewPodcastEpisodeDto(item))
            .OrderBy(dto => dto.PublishDate).ToList();
        PodcastImageUrl = feed.ImageUrl?.ToString() ?? "";
    }
}
