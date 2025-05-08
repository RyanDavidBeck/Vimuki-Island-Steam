using System.Collections.Generic;
using UnityEngine;

namespace ColorDebugLog
{

    /// <summary>
    /// Let create the user the list of debug types defined in <see cref="ColorDebug"/>
    /// </summary>
    [CreateAssetMenu(fileName = "DebugSettings", menuName = "Debug/DebugSettings")]
    public class DebugSettingsSo : ScriptableObject
    {
        public List<DebugTypeConfig> debugConfigs = new();

        public DebugTypeConfig GetConfig(DebugType type)
        {
            return debugConfigs.Find(cfg => cfg.type == type);
        }
    }
    
    [System.Serializable]
    public class DebugTypeConfig
    {
        public DebugType type;
        public bool isEnabled;
        public Color color;
    }
}