using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LiveOpsKit.Editor
{
    public class ReleaseCheckerWindow : EditorWindow
    {
        List<CheckResult> results = new List<CheckResult>();
        Vector2 scroll;

        [MenuItem("Tools/LiveOpsKit/Release Checker")]
        static void Open() => GetWindow<ReleaseCheckerWindow>("Release Checker");

        void OnEnable() => RunChecks();

        void RunChecks() => results = ReleaseCheckRunner.RunAll(ReleaseSettings.LoadOrCreate());

        void OnGUI()
        {
            DrawToolbar();
            DrawSummary();

            scroll = EditorGUILayout.BeginScrollView(scroll);
            foreach (var group in results.GroupBy(r => r.Check))
                DrawCheck(group.Key, group.ToList());
            EditorGUILayout.EndScrollView();
        }

        void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("Run checks", EditorStyles.toolbarButton)) RunChecks();
                if (GUILayout.Button("Settings", EditorStyles.toolbarButton)) Selection.activeObject = ReleaseSettings.LoadOrCreate();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Mark as released", EditorStyles.toolbarButton)) MarkAsReleased();
            }
        }

        void DrawSummary()
        {
            int errors = results.Count(r => r.Severity == Severity.Error);
            int warnings = results.Count(r => r.Severity == Severity.Warning);
            string target = ActiveTarget.Target.ToString();

            if (errors > 0) EditorGUILayout.HelpBox($"{target}: {errors} error(s), {warnings} warning(s). Release builds are blocked.", MessageType.Error);
            else if (warnings > 0) EditorGUILayout.HelpBox($"{target}: ready to build, {warnings} warning(s) to review.", MessageType.Warning);
            else EditorGUILayout.HelpBox($"{target}: ready for a release build.", MessageType.Info);
        }

        void DrawCheck(string check, List<CheckResult> checkResults)
        {
            EditorGUILayout.LabelField(check, EditorStyles.boldLabel);
            foreach (var result in checkResults)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label(Icon(result.Severity), GUILayout.Width(20), GUILayout.Height(18));
                    EditorGUILayout.LabelField(result.Message, EditorStyles.wordWrappedLabel);
                    if (result.Fix != null && GUILayout.Button("Fix", GUILayout.Width(50)))
                    {
                        result.Fix();
                        RunChecks();
                        GUIUtility.ExitGUI();
                    }
                }
            }
            EditorGUILayout.Space();
        }

        void MarkAsReleased()
        {
            string version = PlayerSettings.bundleVersion;
            if (!EditorUtility.DisplayDialog("Mark as released", $"Record version {version} as the last shipped release?", "Record", "Cancel")) return;

            ReleaseSettings.LoadOrCreate().RecordCurrentVersion();
            RunChecks();
        }

        static GUIContent Icon(Severity severity)
        {
            switch (severity)
            {
                case Severity.Error: return EditorGUIUtility.IconContent("console.erroricon.sml");
                case Severity.Warning: return EditorGUIUtility.IconContent("console.warnicon.sml");
                default: return EditorGUIUtility.IconContent("TestPassed");
            }
        }
    }
}
