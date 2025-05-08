using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace ColorDebugLog
{

    [CustomEditor(typeof(DebugSettingsSo))]
    public class DebugSettingsSoEditor : Editor
    {
        private SerializedProperty _debugConfigs;
        private Dictionary<DebugType, bool> _foldouts;

        private void OnEnable()
        {
            _debugConfigs = serializedObject.FindProperty(nameof(DebugSettingsSo.debugConfigs));
            _foldouts = new Dictionary<DebugType, bool>();
            SyncFoldoutDict();
        }

        public override void OnInspectorGUI() 
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debug Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Draw each DebugTypeConfig
            for (var i = 0; i < _debugConfigs.arraySize; i++)
            {
                var config = _debugConfigs.GetArrayElementAtIndex(i);
                var typeProp = config.FindPropertyRelative(nameof(DebugTypeConfig.type));
                var isEnabledProp = config.FindPropertyRelative(nameof(DebugTypeConfig.isEnabled));
                var colorProp = config.FindPropertyRelative(nameof(DebugTypeConfig.color));

                var type = (DebugType)typeProp.enumValueIndex;

                _foldouts.TryAdd(type, true);

                EditorGUILayout.BeginVertical("box");
                _foldouts[type] = EditorGUILayout.Foldout(_foldouts[type], type.ToString(), true);

                if (_foldouts[type])
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(isEnabledProp, new GUIContent("Debug Mode enabled:"));
                    EditorGUILayout.PropertyField(colorProp, new GUIContent("Color"));
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Auto-Generate Debug Types"))
            {
                GenerateMissingDebugTypes((DebugSettingsSo)target);
            }
        
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Expand All"))
            {
                SetAllFoldouts(true);
            }
            if (GUILayout.Button("Collapse All"))
            {
                SetAllFoldouts(false);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (GUILayout.Button("Add Random Colors"))
            {
                AssignRandomColors((DebugSettingsSo)target);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void GenerateMissingDebugTypes(DebugSettingsSo settings)
        {
            var allTypes = System.Enum.GetValues(typeof(DebugType)).Cast<DebugType>().ToList();

            // Remove all obsolete DebugConfigs that no longer exist in the enum
            settings.debugConfigs.RemoveAll(config => !allTypes.Contains(config.type));

            // Add missing types
            foreach (var type in allTypes.Where(type => !settings.debugConfigs.Exists(c => c.type == type)))
            {
                settings.debugConfigs.Add(new DebugTypeConfig
                {
                    type = type,
                    isEnabled = true,
                    color = Color.white
                });
            }

            EditorUtility.SetDirty(settings);
            SyncFoldoutDict();
            serializedObject.Update();
            Repaint();
        }
    
        private void SyncFoldoutDict()
        {
            for (int i = 0; i < _debugConfigs.arraySize; i++)
            {
                var config = _debugConfigs.GetArrayElementAtIndex(i);
                var type = (DebugType)config.FindPropertyRelative(nameof(DebugTypeConfig.type)).enumValueIndex;

                _foldouts.TryAdd(type, true);
            }
        }
    
        private void SetAllFoldouts(bool expanded)
        {
            foreach (var key in _foldouts.Keys.ToList())
            {
                _foldouts[key] = expanded; 
            }

            serializedObject.Update();
            Repaint();
        }

        private void AssignRandomColors(DebugSettingsSo settings)
        {
            var random = new System.Random();

            foreach (var config in settings.debugConfigs.Where(config => config.color == Color.white))
            {
                config.color = new Color(
                    (float)random.NextDouble(),
                    (float)random.NextDouble(),
                    (float)random.NextDouble()
                );
            }

            EditorUtility.SetDirty(settings);
            serializedObject.Update();
            Repaint();
        }
    }

}