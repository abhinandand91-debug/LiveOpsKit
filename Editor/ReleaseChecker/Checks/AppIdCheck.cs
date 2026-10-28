using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;

namespace LiveOpsKit.Editor
{
    public class AppIdCheck : ReleaseCheck
    {
        public override string Name => "App id";

        public override IEnumerable<CheckResult> Run(ReleaseSettings settings)
        {
            if (ActiveTarget.IsAndroid) yield return Compare(NamedBuildTarget.Android, settings.expectedAndroidId);
            else if (ActiveTarget.IsIos) yield return Compare(NamedBuildTarget.iOS, settings.expectedIosId);
            else yield return CheckResult.Pass("App id is only checked for Android and iOS.");
        }

        static CheckResult Compare(NamedBuildTarget target, string expected)
        {
            string actual = PlayerSettings.GetApplicationIdentifier(target);

            if (string.IsNullOrEmpty(expected))
                return CheckResult.Warning($"No expected {target.TargetName} id in Release Settings. Current is '{actual}'.");

            if (actual != expected)
                return CheckResult.Error($"App id is '{actual}' but should be '{expected}'.",
                    () => PlayerSettings.SetApplicationIdentifier(target, expected));

            return CheckResult.Pass($"App id is '{actual}'.");
        }
    }
}
