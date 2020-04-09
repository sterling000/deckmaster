#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


namespace deckmaster
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class GnarlyMenuItems
    {
        private const string MenuName = "Gnarly/Load JSON from Resources";
        private const string SettingName = "load-json-from-resources";

        public static bool IsEnabled
        {
#if UNITY_EDITOR
            get { return EditorPrefs.GetInt(SettingName) == 1; }
            set { EditorPrefs.SetInt(SettingName, value ? 1 : 0); }
#else
            get { return PlayerPrefs.GetInt(SettingName) == 1; }
            set { PlayerPrefs.SetInt(SettingName, value?1:0); }
#endif
        }
#if UNITY_EDITOR
        [MenuItem(MenuName)]
#endif
        private static void ToggleAction()
        {
            IsEnabled = !IsEnabled;
        }
#if UNITY_EDITOR
        [MenuItem(MenuName, true)]
#endif
        private static bool ToggleActionValidate()
        {
#if UNITY_EDITOR
            Menu.SetChecked(MenuName, IsEnabled);
#endif
            return true;
        }
    }
}
