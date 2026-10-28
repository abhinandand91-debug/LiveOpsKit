using UnityEditor;
using UnityEngine;

namespace LiveOpsKit.Editor
{
    // Project rules that a release build must follow.
    public class ReleaseSettings : ScriptableObject
    {
        public const string AssetPath = "Assets/LiveOpsKit/ReleaseSettings.asset";

        [Tooltip("Stop the build when any check reports an error.")]
        public bool blockBuildOnErrors = true;

        [Header("App identity")]
        public string expectedAndroidId = "";
        public string expectedIosId = "";

        [Header("Last released version")]
        public string lastReleasedVersion = "";
        public int lastReleasedAndroidCode;
        public string lastReleasedIosBuild = "";

        [Header("Debug code")]
        public string[] forbiddenDefines = { "DEBUG_MENU", "CHEATS", "ENABLE_CHEATS" };

        [Header("Forbidden text in shipped files")]
        public string[] scanFolders = { "Assets/Resources", "Assets/StreamingAssets" };
        public string[] forbiddenPatterns = { @"staging\.", "localhost", @"""testMode""\s*:\s*true" };

        public static ReleaseSettings Load() => AssetDatabase.LoadAssetAtPath<ReleaseSettings>(AssetPath);

        public static ReleaseSettings LoadOrCreate()
        {
            var settings = Load();
            if (settings != null) return settings;

            settings = CreateInstance<ReleaseSettings>();
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(AssetPath));
            AssetDatabase.CreateAsset(settings, AssetPath);
            AssetDatabase.SaveAssets();
            return settings;
        }
    }
}
