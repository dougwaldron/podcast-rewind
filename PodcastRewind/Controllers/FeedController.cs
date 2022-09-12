using Microsoft.AspNetCore.Mvc;
using PodcastRewind.Models;
using PodcastRewind.Services;

namespace PodcastRewind.Controllers;

[ApiController]
[Route("feed")]
public class FeedController : ControllerBase
{
    private readonly IFeedRewindRepository _repository;
    public FeedController(IFeedRewindRepository repository) => _repository = repository;

    [HttpGet("{id}")]
    [ResponseCache(Duration = 1200)]
    public async Task<IActionResult> GetFeedAsync(Guid? id)
    {
        if (id is null) return Problem($"Feed ID missing.", statusCode: 404);
        var feed = await _repository.GetAsync(id.Value);
        if (feed is null) return Problem($"Feed ID '{id}' not found.", statusCode: 404);

        // TODO: If problems exist in feed rewind -- e.g., missing key, bad dates -- 
        // return single feed entry with info and link to the website.

        var feedPage = Url.PageLink("/Details", values: new { id })!;
        var feedRewindData = new FeedRewindData(feed, feedPage);
        return File(feedRewindData.GetRewoundFeed(), "application/rss+xml; charset=utf-8");
    }
}
