namespace Nop.Plugin.Widgets.NopwaraMegaMenu.Domain;

/// <summary>
/// Link checks for admin-entered URLs that the storefront renders as href
/// </summary>
public static class MegaMenuUrls
{
    /// <summary>
    /// Gets whether the URL is a site-relative path ("/sale") or an absolute http(s) URL; rejects
    /// "javascript:", "data:" and protocol-relative ("//host") links
    /// </summary>
    public static bool IsSafe(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true;

        url = url.Trim();
        if (url.StartsWith('/'))
            return !url.StartsWith("//") && !url.StartsWith("/\\");

        return Uri.TryCreate(url, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
