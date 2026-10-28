using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace LiveOpsKit.Editor
{
    public class BuildScenesCheck : ReleaseCheck
    {
        public override string Name => "Build scenes";

        public override IEnumerable<CheckResult> Run(ReleaseSettings settings)
        {
            var enabled = EditorBuildSettings.scenes.Where(s => s.enabled).ToList();

            if (enabled.Count == 0)
                yield return CheckResult.Error("No scenes are enabled in Build Settings.");

            foreach (var scene in enabled.Where(s => !File.Exists(s.path)))
                yield return CheckResult.Error($"Scene '{scene.path}' is in the build but the file is missing.", () => RemoveScene(scene.path));

            if (enabled.Count > 0 && enabled.All(s => File.Exists(s.path)))
                yield return CheckResult.Pass($"{enabled.Count} scene(s) in the build.");
        }

        static void RemoveScene(string path) =>
            EditorBuildSettings.scenes = EditorBuildSettings.scenes.Where(s => s.path != path).ToArray();
    }
}
