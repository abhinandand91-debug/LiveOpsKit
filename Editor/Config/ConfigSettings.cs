using UnityEditor;
using UnityEngine;

namespace LiveOpsKit.Editor
{
    // Where to load the LiveOps config from and the limits it must respect.
    public class ConfigSettings : ScriptableObject
    {
        public const string AssetPath = "Assets/LiveOpsKit/ConfigSettings.asset";

        [Header("Source")]
        public string configUrl = "";
        public string configFile = "";

        [Header("Limits")]
        public int maxEventDays = 30;
        public int maxRewardAmount = 100000;

        public static ConfigSettings LoadOrCreate()
        {
            var settings = AssetDatabase.LoadAssetAtPath<ConfigSettings>(AssetPath);
            if (settings != null) return settings;

            settings = CreateInstance<ConfigSettings>();
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(AssetPath));
            AssetDatabase.CreateAsset(settings, AssetPath);
            AssetDatabase.SaveAssets();
            return settings;
        }
    }
}
