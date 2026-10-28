using System;
using System.Collections.Generic;
using System.Globalization;
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

        // Event states change as game time moves, so keep repainting.
        void OnInspectorUpdate() => Repaint();

        void OnGUI()
        {
            DrawClock();
            DrawSource();
            if (loadError != null) EditorGUILayout.HelpBox(loadError, MessageType.Error);
            if (json == null) return;

            scroll = EditorGUILayout.BeginScrollView(scroll);
            DrawIssues();
            DrawEvents();
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

        void DrawClock()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                string travel = LiveOpsClock.IsTimeTravelling ? "  (time travelling)" : "";
                GUILayout.Label($"Game time: {Format(LiveOpsClock.UtcNow)} UTC{travel}");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Time Travel", EditorStyles.toolbarButton)) TimeTravelWindow.Open();
            }
        }

        void DrawEvents()
        {
            if (config == null || config.events.Count == 0) return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
            var now = LiveOpsClock.UtcNow;

            foreach (var evt in config.events)
            {
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    var state = evt.StateAt(now);
                    GUILayout.Label(state.ToString(), EditorStyles.boldLabel, GUILayout.Width(70));
                    GUILayout.Label($"{evt.id}  ({evt.type})", GUILayout.MinWidth(120));
                    GUILayout.Label($"{evt.startUtc}  to  {evt.endUtc}", EditorStyles.miniLabel);
                    GUILayout.FlexibleSpace();

                    using (new EditorGUI.DisabledScope(state == EventState.Invalid))
                        DrawJumpButtons(evt);
                }
            }
        }

        // Jumps to the moments where event bugs usually hide: the start and the last minute.
        static void DrawJumpButtons(LiveOpsEvent evt)
        {
            if (!evt.TryGetWindow(out var start, out var end)) return;

            if (GUILayout.Button("Start", GUILayout.Width(50))) LiveOpsClock.TravelTo(start);
            if (GUILayout.Button("Last min", GUILayout.Width(64))) LiveOpsClock.TravelTo(end.AddMinutes(-1));
            if (GUILayout.Button("Ended", GUILayout.Width(52))) LiveOpsClock.TravelTo(end.AddSeconds(1));
        }

        static string Format(DateTime utc) => utc.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

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
