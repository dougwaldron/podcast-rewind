using System.Text.RegularExpressions;

namespace PodcastRewind.Models.DTOs;

public class EditFeedRewindDto : CreateFeedRewindDto
{
    public Guid Id { get; init; }

    public string NormalizedId => Regex.Replace(KeyEntryId, "([^a-zA-Z0-9])", "-");
}
