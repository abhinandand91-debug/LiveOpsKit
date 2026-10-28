using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace LiveOpsKit.Editor
{
    public static class ReleaseCheckRunner
    {
        public static List<CheckResult> RunAll(ReleaseSettings settings)
        {
            var results = new List<CheckResult>();
            foreach (var check in CreateChecks())
                results.AddRange(RunSafely(check, settings));
            return results;
        }

        public static bool HasErrors(IEnumerable<CheckResult> results) =>
            results.Any(r => r.Severity == Severity.Error);

        static IEnumerable<ReleaseCheck> CreateChecks() =>
            TypeCache.GetTypesDerivedFrom<ReleaseCheck>()
                .Where(t => !t.IsAbstract)
                .Select(t => (ReleaseCheck)Activator.CreateInstance(t))
                .OrderBy(c => c.Name);

        static List<CheckResult> RunSafely(ReleaseCheck check, ReleaseSettings settings)
        {
            List<CheckResult> results;
            try
            {
                results = check.Run(settings).ToList();
            }
            catch (Exception e)
            {
                // A broken check should be visible, never silently skipped.
                results = new List<CheckResult> { CheckResult.Error($"Check crashed: {e.Message}") };
            }

            foreach (var result in results) result.Check = check.Name;
            return results;
        }
    }
}
