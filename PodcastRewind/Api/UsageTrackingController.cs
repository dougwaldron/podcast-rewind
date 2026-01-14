using Microsoft.AspNetCore.Mvc;

namespace PodcastRewind.Api;

[ApiController]
[Route("api/usage")]
public class UsageTrackingController(ILogger<UsageTrackingController> logger) : ControllerBase
{
    private static readonly HashSet<string> ValidSubscribeTypes = 
        ["Overcast", "Apple Podcasts", "RSS Link", "RSS Clipboard"];

    [HttpPost("track-subscribe")]
    public IActionResult TrackSubscribe([FromBody] SubscribeTrackingRequest request)
    {
        if (request.FeedId == Guid.Empty || string.IsNullOrEmpty(request.SubscribeType))
            return BadRequest();

        if (!ValidSubscribeTypes.Contains(request.SubscribeType))
            return BadRequest("Invalid subscribe type");

        logger.LogInformation(
            "Subscribe button clicked: FeedId={FeedId}, SubscribeType={SubscribeType}",
            request.FeedId,
            request.SubscribeType);

        return Ok();
    }
}

public class SubscribeTrackingRequest
{
    public required Guid FeedId { get; set; }
    public required string SubscribeType { get; set; }
}
