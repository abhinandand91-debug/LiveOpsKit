using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LiveOpsKit
{
    // Reads ISO 8601 dates from config, like "2026-10-31T18:00:00Z".
    public static class UtcDate
    {
        static readonly Regex ZoneSuffix = new Regex(@"(Z|[+-]\d{2}:?\d{2})$", RegexOptions.IgnoreCase);

        // Dates without "Z" or an offset are read as UTC, but the validator warns about them.
        public static bool TryParse(string text, out DateTime utc)
        {
            var styles = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;
            return DateTime.TryParse(text, CultureInfo.InvariantCulture, styles, out utc);
        }

        public static bool HasTimeZone(string text) => !string.IsNullOrEmpty(text) && ZoneSuffix.IsMatch(text.Trim());
    }
}
