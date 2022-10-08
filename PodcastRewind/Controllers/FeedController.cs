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

    [HttpGet("{id:guid}")]
    [ResponseCache(Duration = 1200)]
    public async Task<IActionResult> GetFeedAsync(Guid? id)
    {
        if (id is null) return Problem($"Feed ID missing.", statusCode: 404);
        var feed = await _repository.GetAsync(id.Value);
        if (feed is null) return Problem($"Feed ID '{id}' not found.", statusCode: 404);

        var feedPage = Url.PageLink("/Details", values: new { id })!;
        var feedRewindData = new FeedRewindData(feed, feedPage);
        return File(await feedRewindData.GetRewoundFeedAsBytesAsync(), FeedRewindData.FeedMimeType);
    }
}
