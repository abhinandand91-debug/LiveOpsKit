using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LiveOpsKit.Editor
{
    // Finds staging urls, test flags and similar text in files that ship with the game.
    public class ForbiddenTextCheck : ReleaseCheck
    {
        static readonly HashSet<string> TextExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".json", ".txt", ".xml", ".csv", ".yaml", ".yml", ".asset", ".ini", ".cfg", ".plist", ".bytes"
        };

        public override string Name => "Forbidden text";

        public override IEnumerable<CheckResult> Run(ReleaseSettings settings)
        {
            var patterns = new List<Regex>();
            foreach (string pattern in settings.forbiddenPatterns)
            {
                if (TryCreateRegex(pattern, out var regex)) patterns.Add(regex);
                else yield return CheckResult.Error($"Pattern '{pattern}' is not a valid regex.");
            }

            var findings = ShippedFiles(settings.scanFolders).SelectMany(file => Scan(file, patterns)).ToList();
            if (findings.Count == 0)
                yield return CheckResult.Pass($"No forbidden text in {string.Join(", ", settings.scanFolders)}.");

            foreach (string finding in findings)
                yield return CheckResult.Error(finding);
        }

        public static IEnumerable<string> Scan(string file, IEnumerable<Regex> patterns)
        {
            string[] lines = File.ReadAllLines(file);
            foreach (var regex in patterns)
            {
                int index = Array.FindIndex(lines, line => regex.IsMatch(line));
                if (index >= 0) yield return $"'{regex}' found in {file} (line {index + 1}).";
            }
        }

        static IEnumerable<string> ShippedFiles(IEnumerable<string> folders) =>
            folders.Where(Directory.Exists)
                .SelectMany(folder => Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories))
                .Where(file => TextExtensions.Contains(Path.GetExtension(file)))
                .Select(file => file.Replace('\\', '/'));

        static bool TryCreateRegex(string pattern, out Regex regex)
        {
            try { regex = new Regex(pattern, RegexOptions.IgnoreCase); return true; }
            catch (ArgumentException) { regex = null; return false; }
        }
    }
}
