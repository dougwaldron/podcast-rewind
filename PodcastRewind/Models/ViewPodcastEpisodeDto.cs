using System.ComponentModel.DataAnnotations;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;

namespace PodcastRewind.Models;

public class ViewPodcastEpisodeDto
{
    public ViewPodcastEpisodeDto(SyndicationItem item)
    {
        Id = item.Id;
        Title = item.Title.Text;
        PublishDate = item.PublishDate;
    }

    public string Id { get; init; }
    public string NormalizedId => Regex.Replace(Id, "([^a-zA-Z0-9])", "-");
    public string Title { get; init; }

    [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
    public DateTimeOffset PublishDate { get; init; }
}
