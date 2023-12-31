using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using PodcastRewind.Models;
using PodcastRewind.Services;

namespace PodcastRewind.Api;

[ApiController]
[Route("feed/{id:guid}")]
public class FeedController(IFeedRewindDataService feedService)
    : ControllerBase
{
    [HttpGet, HttpHead, Produces(FeedRewindData.FeedMimeType)]
    [ResponseCache(Duration = 1200)]
    public async Task<IActionResult> GetAsync(Guid? id)
    {
        if (id is null) return Problem($"Feed ID missing.", statusCode: 404);

        var feedPageLink = Url.PageLink("/Details", values: new { id })!;
        var feedRewindData = await feedService.GetFeedRewindDataAsync(id.Value, feedPageLink);
        if (feedRewindData == null) return NotFound($"Feed ID '{id}' not found.");

        var lastModifiedDateTime = feedRewindData.GetLastModifiedDate();
        var eTag = feedRewindData.GetETag();

        return FeedUnmodified(HttpContext.Request.Headers, eTag, lastModifiedDateTime)
            ? new StatusCodeResult(StatusCodes.Status304NotModified)
            : File(await feedRewindData.GetRewoundFeedAsBytesAsync(), FeedRewindData.FeedMimeType,
                lastModifiedDateTime, new EntityTagHeaderValue(eTag));
    }

    private static bool FeedUnmodified(IHeaderDictionary headers, string eTag, DateTimeOffset lastModifiedDate) =>
        EtagMatches(headers, eTag) || ModifiedSinceHeaderMatches(headers, lastModifiedDate);

    private static bool EtagMatches(IHeaderDictionary headers, string eTag) =>
        headers.TryGetValue(HeaderNames.IfNoneMatch, out var header) &&
        header.ToString().Equals(eTag);

    private static bool ModifiedSinceHeaderMatches(IHeaderDictionary headers, DateTimeOffset lastModifiedDate) =>
        headers.TryGetValue(HeaderNames.IfModifiedSince, out var value) &&
        DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, out var ifModifiedSince) &&
        lastModifiedDate <= ifModifiedSince;
}
