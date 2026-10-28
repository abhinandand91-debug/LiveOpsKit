using UnityEditor;
using UnityEngine;

namespace LiveOpsKit.Editor
{
    public static class SeverityIcon
    {
        public static GUIContent For(Severity severity)
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
