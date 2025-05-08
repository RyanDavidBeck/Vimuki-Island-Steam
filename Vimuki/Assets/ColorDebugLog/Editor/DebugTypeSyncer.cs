using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace ColorDebugLog
{
    /// <summary>
    /// Synchronizes the predefined debug types in <see cref="ColorDebug"/> with the created DebugSettings after loading a script or the project
    /// </summary>
    [InitializeOnLoad]
    public static class DebugTypeSyncer
    {
        static DebugTypeSyncer()
        {
            EditorApplication.delayCall += AutoSyncOnLoad;
        }

        private static void AutoSyncOnLoad()
        {
            var settings = LoadOrCreateSettingsAsset();

            if (settings != null)
            {
                SyncDebugTypes(settings);
            }
        }

        private static DebugSettingsSo LoadOrCreateSettingsAsset()
        {
            var settings = AssetDatabase.LoadAssetAtPath<DebugSettingsSo>(ColorDebug.AssetPath);

            if (settings != null) return settings;
        
            // Create folder if not existing
            var folderPath = Path.GetDirectoryName(ColorDebug.AssetPath);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath!);
                AssetDatabase.Refresh();
            }

            // Create new Asset
            settings = ScriptableObject.CreateInstance<DebugSettingsSo>();
            AssetDatabase.CreateAsset(settings, ColorDebug.AssetPath);
            AssetDatabase.SaveAssets();
            Debug.Log($"[DebugTypeSyncer] New scriptable object >DebugSettings< created under: {ColorDebug.AssetPath}");

            return settings;
        } 
        
        private static void SyncDebugTypes(DebugSettingsSo settings)
        {
            var allTypes = System.Enum.GetValues(typeof(DebugType)).Cast<DebugType>().ToList();
        
            settings.debugConfigs.RemoveAll(cfg => !allTypes.Contains(cfg.type));
        
            foreach (var type in allTypes.Where(type => settings.debugConfigs.All(cfg => cfg.type != type)))
            {
                settings.debugConfigs.Add(new DebugTypeConfig
                {
                    type = type,
                    isEnabled = true,
                    color = Color.white
                });
            }

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }

    }
}