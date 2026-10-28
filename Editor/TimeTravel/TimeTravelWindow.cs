using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace LiveOpsKit.Editor
{
    public class TimeTravelWindow : EditorWindow
    {
        string targetDate = "";

        [MenuItem("Tools/LiveOpsKit/Time Travel")]
        public static void Open() => GetWindow<TimeTravelWindow>("Time Travel");

        // Keep the clocks ticking while the window is open.
        void OnInspectorUpdate() => Repaint();

        void OnGUI()
        {
            DrawClocks();
            EditorGUILayout.Space();
            DrawJumpButtons();
            EditorGUILayout.Space();
            DrawTravelToDate();
        }

        void DrawClocks()
        {
            EditorGUILayout.LabelField("Real time (UTC)", Format(DateTime.UtcNow));
            EditorGUILayout.LabelField("Game time (UTC)", Format(LiveOpsClock.UtcNow), EditorStyles.boldLabel);

            if (!LiveOpsClock.IsTimeTravelling) return;

            EditorGUILayout.HelpBox($"Time travelling by {FormatOffset(LiveOpsClock.Offset)}.", MessageType.Warning);
            if (GUILayout.Button("Back to real time")) LiveOpsClock.BackToRealTime();
        }

        static void DrawJumpButtons()
        {
            DrawJumpRow("-", -1);
            DrawJumpRow("+", 1);
        }

        static void DrawJumpRow(string sign, int direction)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button($"{sign}1 hour")) LiveOpsClock.TravelBy(TimeSpan.FromHours(direction));
                if (GUILayout.Button($"{sign}1 day")) LiveOpsClock.TravelBy(TimeSpan.FromDays(direction));
                if (GUILayout.Button($"{sign}7 days")) LiveOpsClock.TravelBy(TimeSpan.FromDays(7 * direction));
            }
        }

        void DrawTravelToDate()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                targetDate = EditorGUILayout.TextField("Go to (UTC)", targetDate);
                if (GUILayout.Button("Go", GUILayout.Width(40))) TravelToTypedDate();
            }
            EditorGUILayout.LabelField("Example: 2026-12-31 23:59", EditorStyles.miniLabel);
        }

        void TravelToTypedDate()
        {
            var styles = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;
            if (DateTime.TryParse(targetDate, CultureInfo.InvariantCulture, styles, out var utc))
                LiveOpsClock.TravelTo(utc);
            else
                ShowNotification(new GUIContent("Could not read that date"));
        }

        static string Format(DateTime utc) => utc.ToString("ddd yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        static string FormatOffset(TimeSpan offset)
        {
            string sign = offset < TimeSpan.Zero ? "-" : "+";
            var size = offset.Duration();
            return $"{sign}{size.Days}d {size.Hours}h {size.Minutes}m";
        }
    }
}
