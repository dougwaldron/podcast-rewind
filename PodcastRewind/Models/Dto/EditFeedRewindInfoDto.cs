namespace PodcastRewind.Models.Dto;

public class EditFeedRewindInfoDto : CreateFeedRewindInfoDto
{
    public Guid Id { get; init; }

    public string NormalizedId => RegexPatterns.NonAlphanumeric().Replace(KeyEntryId, "-");
}
