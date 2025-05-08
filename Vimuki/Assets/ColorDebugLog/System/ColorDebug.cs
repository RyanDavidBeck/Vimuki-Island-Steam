using UnityEngine;

namespace ColorDebugLog
{

    public static class ColorDebug
    {
        private static DebugSettingsSo _settings;

        public const string AssetPath = "Assets/Resources/DebugSettings.asset";

        private static DebugSettingsSo Settings
        {
            get
            {
                if (_settings == null)
                {
#if UNITY_EDITOR
                    _settings = UnityEditor.AssetDatabase.LoadAssetAtPath<DebugSettingsSo>(AssetPath);
#else
                _settings = Resources.Load<DebugSettingsSO>("DebugSettings");
#endif
                }
                return _settings;
            }
        }

        /// <summary>
        /// Logs the corresponding message in the colored highlight that was defined via DebugSettings. 
        /// </summary>
        /// <param name="message">The debug log message</param>
        /// <param name="type">The DebugType predefined in the <see cref="DebugSettingsSo"/></param>
        public static void Log(string message, DebugType type)
        {
            var config = Settings?.GetConfig(type);

            if (config is not { isEnabled: true })
                return;

            var colorHex = ColorUtility.ToHtmlStringRGB(config.color);
            Debug.Log($"<color=#{colorHex}>{type}: {message}</color>");
        }

    }
}