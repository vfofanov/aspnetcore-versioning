namespace Stenn.AspNetCore.Versioning
{
    public static class VersioningRoutingPrefixHelper
    {
        public static string GeneratePrefix(string prefixFormatTemplate, ApiVersionInfo v)
        {
            return "/" + string.Format(prefixFormatTemplate, v.RoutePathName).Trim('/');
        }
    }
}