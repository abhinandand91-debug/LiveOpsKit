using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace LiveOpsKit.Editor
{
    public class DefineSymbolsCheck : ReleaseCheck
    {
        public override string Name => "Debug defines";

        public override IEnumerable<CheckResult> Run(ReleaseSettings settings)
        {
            var target = ActiveTarget.Named;
            var found = GetDefines(target).Intersect(settings.forbiddenDefines).ToList();

            if (found.Count == 0)
            {
                yield return CheckResult.Pass("No debug defines are set.");
                yield break;
            }

            foreach (string define in found)
                yield return CheckResult.Error($"Debug define '{define}' is set for {target.TargetName}.", () => RemoveDefine(target, define));
        }

        static string[] GetDefines(NamedBuildTarget target) =>
            PlayerSettings.GetScriptingDefineSymbols(target).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        static void RemoveDefine(NamedBuildTarget target, string define)
        {
            var remaining = GetDefines(target).Where(d => d != define);
            PlayerSettings.SetScriptingDefineSymbols(target, string.Join(";", remaining));
        }
    }
}
