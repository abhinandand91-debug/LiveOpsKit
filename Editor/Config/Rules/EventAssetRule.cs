using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LiveOpsKit.Editor
{
    // Asset paths are either full project paths ("Assets/...") or Resources paths ("Events/Banner").
    public class EventAssetRule : ConfigRule
    {
        public override IEnumerable<ConfigIssue> Validate(LiveOpsConfig config, ConfigSettings settings, DateTime now)
        {
            foreach (var evt in config.events)
            {
                if (evt.assets == null) continue;

                foreach (string path in evt.assets)
                {
                    if (string.IsNullOrWhiteSpace(path))
                        yield return ConfigIssue.Error(Label(config, evt), "Has an empty asset path.");
                    else if (!Exists(path))
                        yield return ConfigIssue.Error(Label(config, evt), $"Asset '{path}' is not in the project.");
                }
            }
        }

        public static bool Exists(string path) =>
            path.StartsWith("Assets/")
                ? AssetDatabase.LoadMainAssetAtPath(path) != null
                : Resources.Load(path) != null;
    }
}
