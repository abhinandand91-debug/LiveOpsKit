using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace LiveOpsKit.Editor
{
    public class AndroidSigningCheck : ReleaseCheck
    {
        public override string Name => "Android signing";

        public override IEnumerable<CheckResult> Run(ReleaseSettings settings)
        {
            if (!ActiveTarget.IsAndroid)
            {
                yield return CheckResult.Pass("Not building for Android.");
                yield break;
            }

            if (!PlayerSettings.Android.useCustomKeystore)
                yield return CheckResult.Error("Build is signed with the debug keystore. The store will reject it.");
            else if (!File.Exists(PlayerSettings.Android.keystoreName))
                yield return CheckResult.Error($"Keystore not found at '{PlayerSettings.Android.keystoreName}'.");
            else if (string.IsNullOrEmpty(PlayerSettings.Android.keyaliasName))
                yield return CheckResult.Error("Keystore is set but no key alias is selected.");
            else
                yield return CheckResult.Pass($"Signed with alias '{PlayerSettings.Android.keyaliasName}'.");
        }
    }
}
