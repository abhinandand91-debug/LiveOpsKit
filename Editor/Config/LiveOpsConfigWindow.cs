using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LiveOpsKit.Editor
{
    public class LiveOpsConfigWindow : EditorWindow
    {
        ConfigSettings settings;
        string json;
        string loadError;
        LiveOpsConfig config;
        List<ConfigIssue> issues = new List<ConfigIssue>();
        Vector2 scroll;

        [MenuItem("Tools/LiveOpsKit/LiveOps Config")]
        static void Open() => GetWindow<LiveOpsConfigWindow>("LiveOps Config");

        void OnEnable() => settings = ConfigSettings.LoadOrCreate();

        void OnGUI()
        {
            DrawSource();
            if (loadError != null) EditorGUILayout.HelpBox(loadError, MessageType.Error);
            if (json == null) return;

            scroll = EditorGUILayout.BeginScrollView(scroll);
            DrawIssues();
            EditorGUILayout.EndScrollView();
        }

        void DrawSource()
        {
            EditorGUI.BeginChangeCheck();
            using (new EditorGUILayout.HorizontalScope())
            {
                settings.configUrl = EditorGUILayout.TextField("Url", settings.configUrl);
                if (GUILayout.Button("Download", GUILayout.Width(80))) Download();
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                settings.configFile = EditorGUILayout.TextField("File", settings.configFile);
                if (GUILayout.Button("...", GUILayout.Width(24))) BrowseFile();
                if (GUILayout.Button("Load", GUILayout.Width(52))) LoadFile();
            }
            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(settings);

            if (json != null && GUILayout.Button("Validate again")) Validate(json);
        }

        void DrawIssues()
        {
            int errors = issues.Count(i => i.Severity == Severity.Error);
            if (issues.Count == 0) EditorGUILayout.HelpBox("Config looks good.", MessageType.Info);
            else EditorGUILayout.HelpBox($"{errors} error(s), {issues.Count - errors} warning(s).", errors > 0 ? MessageType.Error : MessageType.Warning);

            foreach (var issue in issues)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label(SeverityIcon.For(issue.Severity), GUILayout.Width(20), GUILayout.Height(18));
                    string prefix = string.IsNullOrEmpty(issue.EventId) ? "" : $"{issue.EventId}: ";
                    EditorGUILayout.LabelField(prefix + issue.Message, EditorStyles.wordWrappedLabel);
                }
            }
        }

        void BrowseFile()
        {
            string path = EditorUtility.OpenFilePanel("LiveOps config", Application.dataPath, "json");
            if (string.IsNullOrEmpty(path)) return;

            settings.configFile = path;
            EditorUtility.SetDirty(settings);
            LoadFile();
        }

        void LoadFile()
        {
            if (ConfigLoader.TryReadFile(settings.configFile, out string text, out string error)) Validate(text);
            else ShowLoadError(error);
        }

        void Download()
        {
            loadError = null;
            ShowNotification(new GUIContent("Downloading..."));
            ConfigLoader.Download(settings.configUrl, (text, error) =>
            {
                RemoveNotification();
                if (error == null) Validate(text);
                else ShowLoadError(error);
            });
        }

        void Validate(string text)
        {
            json = text;
            loadError = null;
            issues = ConfigValidator.Validate(text, settings, LiveOpsClock.UtcNow, out config);
            Repaint();
        }

        void ShowLoadError(string error)
        {
            loadError = error;
            json = null;
            config = null;
            Repaint();
        }
    }
}
