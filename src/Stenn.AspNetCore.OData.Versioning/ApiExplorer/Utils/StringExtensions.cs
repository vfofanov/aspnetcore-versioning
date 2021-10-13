using System.Globalization;

namespace Microsoft
{
    internal static class StringExtensions
    {
        internal static string FormatInvariant(this string format, params object?[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        internal static string FormatDefault(this string format, params object?[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }
    }
}