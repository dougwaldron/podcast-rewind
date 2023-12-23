namespace PodcastRewind.Models.Dto;

public class EditFeedRewindDto : CreateFeedRewindDto
{
    public Guid Id { get; init; }

    public string NormalizedId => RegexPatterns.NonAlphanumeric().Replace(KeyEntryId, "-");
}
