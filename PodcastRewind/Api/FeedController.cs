using Microsoft.AspNetCore.Mvc;
using PodcastRewind.Models;
using PodcastRewind.Services;

namespace PodcastRewind.Api;

[ApiController]
[Route("feed/{id:guid}")]
public class FeedController : ControllerBase
{
    private readonly IFeedRewindRepository _repository;
    private readonly ISyndicationFeedService _syndicationFeedService;

    public FeedController(
        IFeedRewindRepository repository,
        ISyndicationFeedService syndicationFeedService)
    {
        _repository = repository;
        _syndicationFeedService = syndicationFeedService;
    }

    [HttpGet, HttpHead]
    [ResponseCache(Duration = 1200)]
    public async Task<IActionResult> GetAsync(Guid? id)
    {
        if (id is null) return Problem($"Feed ID missing.", statusCode: 404);
        var feedRewindInfo = await _repository.GetAsync(id.Value);
        if (feedRewindInfo is null) return Problem($"Feed ID '{id}' not found.", statusCode: 404);

        var originalFeed = await _syndicationFeedService.GetSyndicationFeedAsync(feedRewindInfo.FeedUrl);
        if (originalFeed is null) return NotFound();

        var feedPage = Url.PageLink("/Details", values: new { id })!;
        var feedRewindData = new FeedRewindData(feedRewindInfo, originalFeed, feedPage);
        return File(await feedRewindData.GetRewoundFeedAsBytesAsync(), FeedRewindData.FeedMimeType);
    }
}
