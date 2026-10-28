using System.Collections.Generic;
using UnityEditor;

namespace LiveOpsKit.Editor
{
    public class DevelopmentBuildCheck : ReleaseCheck
    {
        public override string Name => "Development build";

        public override IEnumerable<CheckResult> Run(ReleaseSettings settings)
        {
            if (EditorUserBuildSettings.development)
                yield return CheckResult.Error("Development Build is on.", () => EditorUserBuildSettings.development = false);
            else
                yield return CheckResult.Pass("Development Build is off.");

            if (EditorUserBuildSettings.connectProfiler)
                yield return CheckResult.Warning("Autoconnect Profiler is on.", () => EditorUserBuildSettings.connectProfiler = false);

            if (EditorUserBuildSettings.allowDebugging)
                yield return CheckResult.Warning("Script Debugging is on.", () => EditorUserBuildSettings.allowDebugging = false);
        }
    }
}
