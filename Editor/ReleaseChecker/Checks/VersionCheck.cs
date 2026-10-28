using System;
using System.Collections.Generic;
using UnityEditor;

namespace LiveOpsKit.Editor
{
    public class VersionCheck : ReleaseCheck
    {
        public override string Name => "Version bump";

        public override IEnumerable<CheckResult> Run(ReleaseSettings settings)
        {
            yield return CheckVersionName(PlayerSettings.bundleVersion, settings.lastReleasedVersion);

            if (ActiveTarget.IsAndroid)
                yield return CheckAndroidCode(settings.lastReleasedAndroidCode);

            if (ActiveTarget.IsIos)
                yield return CheckIosBuild(settings.lastReleasedIosBuild);
        }

        public static CheckResult CheckVersionName(string current, string lastReleased)
        {
            if (string.IsNullOrEmpty(lastReleased))
                return CheckResult.Warning("No released version recorded yet. Use 'Mark as released' after you ship.");

            if (!Version.TryParse(current, out var now) || !Version.TryParse(lastReleased, out var last))
                return CheckResult.Warning($"Cannot compare versions '{current}' and '{lastReleased}'.");

            return now > last
                ? CheckResult.Pass($"Version {current} is newer than {lastReleased}.")
                : CheckResult.Error($"Version {current} is not newer than released {lastReleased}.");
        }

        static CheckResult CheckAndroidCode(int lastReleased)
        {
            int current = PlayerSettings.Android.bundleVersionCode;
            if (current > lastReleased)
                return CheckResult.Pass($"Bundle version code {current} is newer than {lastReleased}.");

            return CheckResult.Error($"Bundle version code {current} must be higher than released {lastReleased}.",
                () => PlayerSettings.Android.bundleVersionCode = lastReleased + 1);
        }

        static CheckResult CheckIosBuild(string lastReleased)
        {
            string current = PlayerSettings.iOS.buildNumber;
            return current == lastReleased
                ? CheckResult.Error($"iOS build number {current} was already released.")
                : CheckResult.Pass($"iOS build number is {current}.");
        }
    }
}
