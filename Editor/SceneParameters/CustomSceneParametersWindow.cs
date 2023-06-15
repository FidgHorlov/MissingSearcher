#region Info

// Tsukat tool - by Horlov Andrii (andreygorlovv@gmail.com)
// Tsukat -> https://tsukat.com/

#endregion

using System;
using UnityEditor;
using UnityEngine;

namespace TsukatTool.Editor.SceneParameters
{
    public class CustomSceneParametersWindow : EditorWindow
    {
        private const string SceneManagerPath = "Tsukat/Scene Parameters/Main window";
        private const string PrepareBuildPath = "Tsukat/Scene Parameters/Prepare build";
        private const string SceneManagerSettingsPath = "Tsukat/Scene Parameters/Settings";

        private const string SceneManagerWindowName = "Scene Parameters";
        private const string SettingsWindowName = "Settings";
        private const string PrepareBuildWindowName = "Prepare build";

        private const int WindowSizeMinX = 300;
        private const int WindowSizeMinY = 400;
        private const int WindowSizeMaxX = 600;
        private const int WindowSizeMaxY = 720;

        private const int SettingsWindowSizeYMin = 150;
        private const int SettingsWindowSizeYMax = 600;

        private const int PrepareBuildWindowSizeYMin = 300;
        private const int PrepareBuildWindowSizeYMax = 800;

        private const int PrepareBuildWindowSizeMinX = 600;
        private const int PrepareBuildWindowSizeMaxX = 800;

        [MenuItem(SceneManagerPath, priority = -1000)]
        public static void OpenWindow()
        {
            if (HasOpenInstances<SceneManagerWindow>())
            {
                FocusWindowIfItsOpen<SceneManagerWindow>();
                //return;
            }
            
            EditorWindow window = GetWindow<SceneManagerWindow>();
            window.titleContent = new GUIContent(SceneManagerWindowName);
            window.minSize = new Vector2(WindowSizeMinX, WindowSizeMinY);
            window.maxSize = new Vector2(WindowSizeMaxX, WindowSizeMaxY);
            window.position = new Rect(Screen.currentResolution.width / 2f, Screen.currentResolution.height / 2f, WindowSizeMinX, WindowSizeMinY);
        }

        [MenuItem(PrepareBuildPath, priority = -910)]
        public static void OpenPrepareBuildWindow()
        {
            if (HasOpenInstances<PrepareBuildWindow>())
            {
                FocusWindowIfItsOpen<PrepareBuildWindow>();
                return;
            }
            
            EditorWindow window = GetWindow<PrepareBuildWindow>();
            window.titleContent = new GUIContent(PrepareBuildWindowName);
            window.minSize = new Vector2(PrepareBuildWindowSizeMinX, PrepareBuildWindowSizeYMin);
            window.maxSize = new Vector2(PrepareBuildWindowSizeMaxX, PrepareBuildWindowSizeYMax);
            window.position = new Rect(Screen.currentResolution.width / 2f, Screen.currentResolution.height / 2f, PrepareBuildWindowSizeMinX, WindowSizeMinY);
            window.Show(immediateDisplay: true);
        }

        [MenuItem(SceneManagerSettingsPath, priority = -909)]
        public static void OpenSettingsWindow()
        {
            if (HasOpenInstances<SettingsCustomSceneParametersWindow>())
            {
                FocusWindowIfItsOpen<SettingsCustomSceneParametersWindow>();
                return;
            }
            
            EditorWindow window = GetWindow<SettingsCustomSceneParametersWindow>();
            window.titleContent = new GUIContent(SettingsWindowName);
            window.minSize = new Vector2(WindowSizeMinX, SettingsWindowSizeYMin);
            window.maxSize = new Vector2(WindowSizeMaxX, SettingsWindowSizeYMax);
            window.position = new Rect(Screen.currentResolution.width / 2f, Screen.currentResolution.height / 2f, WindowSizeMinX, SettingsWindowSizeYMin);
            window.Show(immediateDisplay: true);
        }
    }
}