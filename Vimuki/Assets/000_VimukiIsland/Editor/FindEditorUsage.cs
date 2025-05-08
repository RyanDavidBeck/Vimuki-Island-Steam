using System.IO;
using UnityEditor;
using UnityEngine;

public class FindEditorUsages : EditorWindow
{
    [MenuItem("Tools/Find Editor Usages in Non-Editor Scripts")]
    static void FindEditorCodeInWrongPlaces()
    {
        var searchPaths = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);

        foreach (var path in searchPaths)
        {
            if (path.Contains("/Editor/") || path.Contains(@"\Editor\")) continue;

            var lines = File.ReadAllLines(path);
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if (line.Contains("UnityEditor") || 
                    line.Contains("EditorGUILayout") ||
                    line.Contains("EditorWindow") ||
                    line.Contains("[CustomEditor") ||
                    line.Contains("using UnityEditor"))
                {
                    Debug.LogWarning($"Editor usage in non-editor script:\n{path.Replace(Application.dataPath, "Assets")} (Line {i + 1}): {line.Trim()}");
                }
            }
        }

        Debug.Log("Editor usage check complete.");
    }
}