using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using PodcastRewind.Models;
using PodcastRewind.Services;
using System.Text;

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

        var eTag = GenerateEtagFromLastModified(feedRewindData.GetLastModifiedDate());
        var headers = HttpContext.Request.Headers;

        if (headers.ContainsKey(HeaderNames.IfNoneMatch))
        {
            var incomingEtag = headers[HeaderNames.IfNoneMatch].ToString();
            if (incomingEtag.Equals(eTag))
                return new StatusCodeResult(StatusCodes.Status304NotModified);
        }
        else if (headers.ContainsKey(HeaderNames.IfModifiedSince) &&
                 DateTimeOffset.TryParse(headers[HeaderNames.IfModifiedSince], out var ifModifiedSince) &&
                 feedRewindData.GetLastModifiedDate() <= ifModifiedSince)
        {
            return new StatusCodeResult(StatusCodes.Status304NotModified);
        }

        return File(await feedRewindData.GetRewoundFeedAsBytesAsync(), FeedRewindData.FeedMimeType,
            feedRewindData.GetLastModifiedDate(), new EntityTagHeaderValue(eTag));
    }

    private static string GenerateEtagFromLastModified(DateTimeOffset lastModifiedDateTime) =>
        "\"" + Convert.ToBase64String(Encoding.UTF8.GetBytes(lastModifiedDateTime.ToString())) + "\"";
}
