using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using PodcastRewind.Models;
using PodcastRewind.Services;

namespace PodcastRewind.Api;

[ApiController]
[Route("feed/{id:guid}")]
public class FeedController(IFeedRewindInfoRepository repository, ISyndicationFeedService feedService)
    : ControllerBase
{
    [HttpGet, HttpHead, Produces(FeedRewindData.FeedMimeType)]
    [ResponseCache(Duration = 1200)]
    public async Task<IActionResult> GetAsync(Guid? id)
    {
        if (id is null) return Problem($"Feed ID missing.", statusCode: 404);
        var feedRewindInfo = await repository.GetAsync(id.Value);
        if (feedRewindInfo is null) return Problem($"Feed ID '{id}' not found.", statusCode: 404);

        var originalFeed = await feedService.GetSyndicationFeedAsync(feedRewindInfo.FeedUrl);
        if (originalFeed is null) return NotFound();

        var feedPage = Url.PageLink("/Details", values: new { id })!;
        var feedRewindData = new FeedRewindData(feedRewindInfo, originalFeed, feedPage);

        var eTag = GenerateEtagFromLastModified(feedRewindData.GetLastModifiedDate());
        var headers = HttpContext.Request.Headers;

        if (headers.TryGetValue(HeaderNames.IfNoneMatch, out var header))
        {
            var incomingEtag = header.ToString();
            if (incomingEtag.Equals(eTag))
                return new StatusCodeResult(StatusCodes.Status304NotModified);
        }
        else if (headers.TryGetValue(HeaderNames.IfModifiedSince, out var value) &&
                 DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, out var ifModifiedSince) &&
                 feedRewindData.GetLastModifiedDate() <= ifModifiedSince)
        {
            return new StatusCodeResult(StatusCodes.Status304NotModified);
        }

        return File(await feedRewindData.GetRewoundFeedAsBytesAsync(), FeedRewindData.FeedMimeType,
            feedRewindData.GetLastModifiedDate(), new EntityTagHeaderValue(eTag));
    }

    private static string GenerateEtagFromLastModified(DateTimeOffset lastModifiedDateTime) =>
        $"\"{Convert.ToBase64String(Encoding.UTF8.GetBytes(lastModifiedDateTime.ToString()))}\"";
}
