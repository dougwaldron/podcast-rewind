using System.Text.RegularExpressions;

namespace PodcastRewind.Models;

public static partial class RegexPatterns
{
    [GeneratedRegex("([^a-zA-Z0-9])")]
    public static partial Regex NonAlphanumeric();
}
