using System.Text.RegularExpressions;

namespace HiSubmit.Application.Services.Text;

/// <summary>
/// Keeps the existing rich-text formatting while preventing links and
/// executable/embed content in user-authored festival text.
/// </summary>
public static partial class HtmlTextSanitizer
{
    public static string SanitizeWithoutLinks(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        var value = DangerousBlockRegex().Replace(html, string.Empty);
        value = AnchorTagRegex().Replace(value, string.Empty);
        value = EventAttributeRegex().Replace(value, string.Empty);
        value = UrlAttributeRegex().Replace(value, string.Empty);
        return value;
    }

    [GeneratedRegex(@"<\s*(script|style|iframe|object|embed|form)\b[^>]*>.*?<\s*/\s*\1\s*>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex DangerousBlockRegex();

    [GeneratedRegex(@"</?\s*a\b[^>]*>", RegexOptions.IgnoreCase)]
    private static partial Regex AnchorTagRegex();

    [GeneratedRegex(@"\s+on[a-z0-9_-]+\s*=\s*(?:""[^""]*""|'[^']*'|[^\s>]+)", RegexOptions.IgnoreCase)]
    private static partial Regex EventAttributeRegex();

    [GeneratedRegex(@"\s+(href|src)\s*=\s*(?:""[^""]*""|'[^']*'|[^\s>]+)", RegexOptions.IgnoreCase)]
    private static partial Regex UrlAttributeRegex();
}
