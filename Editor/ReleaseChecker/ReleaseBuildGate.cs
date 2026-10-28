using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace LiveOpsKit.Editor
{
    // Runs every check before a release build and stops the build on errors.
    public class ReleaseBuildGate : IPreprocessBuildWithReport
    {
        public int callbackOrder => -1000;

        public void OnPreprocessBuild(BuildReport report)
        {
            if ((report.summary.options & BuildOptions.Development) != 0) return;

            var settings = ReleaseSettings.Load();
            if (settings == null || !settings.blockBuildOnErrors) return;

            var results = ReleaseCheckRunner.RunAll(settings);
            foreach (var result in results)
            {
                if (result.Severity == Severity.Error) Debug.LogError($"[LiveOpsKit] {result.Check}: {result.Message}");
                if (result.Severity == Severity.Warning) Debug.LogWarning($"[LiveOpsKit] {result.Check}: {result.Message}");
            }

            if (ReleaseCheckRunner.HasErrors(results))
                throw new BuildFailedException("Release checks failed. Open Tools > LiveOpsKit > Release Checker to fix them.");
        }
    }
}
