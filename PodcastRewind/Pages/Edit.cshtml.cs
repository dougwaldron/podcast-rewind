﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PodcastRewind.Models.DTOs;
using PodcastRewind.Services;
using System.ServiceModel.Syndication;
using System.Xml;

namespace PodcastRewind.Pages;

public class EditModel : PageModel
{
    private readonly IFeedRewindRepository _repository;
    public EditModel(IFeedRewindRepository repository) => _repository = repository;

    [BindProperty]
    public EditFeedRewindDto EditFeedRewind { get; set; } = null!;

    public string PodcastTitle { get; private set; } = string.Empty;
    public List<ViewPodcastEpisodeDto> PodcastEpisodes { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id is null) return RedirectToPage("/Index");
        var feedRewind = await _repository.GetAsync(id.Value);
        if (feedRewind is null) return NotFound($"Feed ID '{id}' not found.");

        using var xmlReader = XmlReader.Create(feedRewind.FeedUrl);
        var feed = SyndicationFeed.Load(xmlReader);
        if (feed is null) return NotFound();

        LoadData(feed);

        if (PodcastEpisodes.Count > 0)
        {
            EditFeedRewind = new EditFeedRewindDto
            {
                Id = feedRewind.Id,
                FeedUrl = feedRewind.FeedUrl,
                KeyEntryId = feedRewind.KeyEntryId,
                DateOfKeyEntry = feedRewind.DateOfKeyEntry,
                Interval = feedRewind.Interval,
            };
        }

        return Page();
    }


    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            using var xmlReader = XmlReader.Create(EditFeedRewind.FeedUrl);
            var feed = SyndicationFeed.Load(xmlReader);
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
    }
}