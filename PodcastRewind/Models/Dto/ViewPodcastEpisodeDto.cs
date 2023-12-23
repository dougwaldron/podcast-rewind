using System.ComponentModel.DataAnnotations;
using System.ServiceModel.Syndication;

namespace PodcastRewind.Models.Dto;

public class ViewPodcastEpisodeDto(SyndicationItem item)
{
    public string Id { get; } = item.Id;
    public string NormalizedId => RegexPatterns.NonAlphanumeric().Replace(Id, "-");
    public string Title { get; } = item.Title.Text;

    [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
    public DateTimeOffset PublishDate { get; init; } = item.PublishDate;
}
