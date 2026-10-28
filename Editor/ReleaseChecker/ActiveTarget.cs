using UnityEditor;
using UnityEditor.Build;

namespace LiveOpsKit.Editor
{
    // The build target the project is currently switched to.
    public static class ActiveTarget
    {
        public static BuildTarget Target => EditorUserBuildSettings.activeBuildTarget;
        public static NamedBuildTarget Named => NamedBuildTarget.FromBuildTargetGroup(BuildPipeline.GetBuildTargetGroup(Target));
        public static bool IsAndroid => Target == BuildTarget.Android;
        public static bool IsIos => Target == BuildTarget.iOS;
    }
}
