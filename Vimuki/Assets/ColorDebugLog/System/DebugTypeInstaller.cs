using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using System.IO;

namespace ColorDebugLog
{
    public static class DebugTypeInstaller
    {
        private const string Define = "COLOR_DEBUG_CUSTOM_TYPES";
        private const string TargetFolderPath = "Assets/ColorDebug";
        private const string TargetFilePath = TargetFolderPath + "/DebugType.cs";
        private const string TemplateFilePathInPackage = "Packages/Color Debug Log/Templates/DebugTypeTemplate.txt";
        
        [MenuItem("Tools/ColorDebug/Install DebugTypes")]
        public static void Install()
        {
            if (!Directory.Exists(TargetFolderPath))
            {
                Directory.CreateDirectory(TargetFolderPath);
                AssetDatabase.Refresh();
            }

            if (!File.Exists(TemplateFilePathInPackage))
            {
                Debug.LogError("[ColorDebug] Template is missing: " + TemplateFilePathInPackage);
                return;
            }
            
            var templateContent = File.ReadAllText(TemplateFilePathInPackage);
            File.WriteAllText(TargetFilePath, templateContent);

            AssetDatabase.Refresh();

            ApplyDefineIfMissing(BuildTargetGroup.Standalone);
            ApplyDefineIfMissing(BuildTargetGroup.Android);
            ApplyDefineIfMissing(BuildTargetGroup.iOS);
        }

        private static void ApplyDefineIfMissing(BuildTargetGroup group)
        {
            var namedTarget = NamedBuildTarget.FromBuildTargetGroup(group);
            var defines = PlayerSettings.GetScriptingDefineSymbols(namedTarget);

            if (defines.Contains(Define)) return;
            
            var newDefines = string.IsNullOrWhiteSpace(defines) ? Define : $"{defines};{Define}";
            PlayerSettings.SetScriptingDefineSymbols(namedTarget, newDefines);
            Debug.Log($"[ColorDebug] Added define '{Define}' to {group}");
        }
    }
}
