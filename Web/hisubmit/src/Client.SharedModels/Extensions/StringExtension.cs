namespace HiSubmit.Client.SharedModels.Extensions;

public static class StringExtension
{
    public static string GenerateStyleUrl(this string url)
    {
        var f = url.Replace(@"\", @"/");
        return f;
    }

    public static string GetMimType(this string fileName)
    {
     //   var mimeType = MimeMapping.MimeUtility.GetMimeMapping(fileName);
        return "";
    }

    public static string ToAbsolutUrl(this  string url)
    {
        if (url.StartsWith("https://") || url.StartsWith("http://"))
            return url;

        return $"https://{url}";
    }
    public static string TrimAll(this string url)
    {
        return url.Replace(" ", "");
    }
}