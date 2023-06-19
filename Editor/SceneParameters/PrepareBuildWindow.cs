#region Info

// Tsukat tool - by Horlov Andrii (andreygorlovv@gmail.com)
// Tsukat -> https://tsukat.com/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using TsukatTool.Editor.SceneParameters.Models;
using TsukatTool.Editor.SceneParameters.Utilities;
using UnityEditor;
using UnityEngine;

namespace TsukatTool.Editor.SceneParameters
{
    [Serializable]
    internal class SelectedScene
    {
        public SceneData SceneData { get; set; }
        public bool IsSelected { get; set; }
    }

    public class PrepareBuildWindow : EditorWindow
    {
        private const string Header = "Here you can switch between platforms, and make final adjustment before go to the Build Settings";
        private const string ButtonName = "Open Build Settings";
        private const string Standalone64WarningMsg = "Unfortunately, we can't detect what is Standalone Windows and Windows 64, please select the proper scenes by yourself";

        private BuildTargetGroup _lastSelectedGroup;
        private string _lastSelectedStandaloneTarget;
        private List<SelectedScene> _selectedScenes;

        private void OnGUI()
        {
            ScenesSettings scenesSettings = FileManager.LoadScenesSettings();
            if (scenesSettings == null)
            {
                Debug.Log($"File with scenes settings is missing");
                return;
            }

            EditorGUILayout.LabelField(Header, EditorStyles.wordWrappedLabel);
            BuildTargetGroup selectedGroup = EditorGUILayout.BeginBuildTargetSelectionGrouping();
            if (selectedGroup == BuildTargetGroup.Standalone)
            {
                ShowStandaloneMessage();
            }

            if (!selectedGroup.Equals(_lastSelectedGroup))
            {
                _selectedScenes = new List<SelectedScene>();
            }

            foreach (SceneData sceneData in scenesSettings.ScenesData)
            {
                if (!sceneData.IsBuildAdded)
                {
                    continue;
                }
                
                foreach (CustomBuildTarget buildTarget in sceneData.TargetPlatformSettings.BuildTargets)
                {
                    BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(Enum.Parse<BuildTarget>(buildTarget.Name));
                    if (!selectedGroup.Equals(targetGroup) || !buildTarget.IsSelected || selectedGroup.Equals(_lastSelectedGroup))
                    {
                        continue;
                    }

                    SelectedScene selectedScene = new SelectedScene {SceneData = sceneData, IsSelected = true};
                    _selectedScenes.Add(selectedScene);
                }
            }
            
            DeleteRepeated();

            foreach (SelectedScene selectedScene in _selectedScenes)
            {
                GenerateToggleSelectedScenes(selectedScene.SceneData);
            }
            
            EditorGUILayout.EndBuildTargetSelectionGrouping();
            _lastSelectedGroup = selectedGroup;

            if (GUILayout.Button(ButtonName))
            {
                List<EditorBuildSettingsScene> settingsScenes = new List<EditorBuildSettingsScene>();
                foreach (SelectedScene sceneData in _selectedScenes)
                {
                    if (sceneData.IsSelected)
                    {
                        settingsScenes.Add(new EditorBuildSettingsScene(sceneData.SceneData.Path, true));
                    }
                }

                EditorBuildSettings.scenes = settingsScenes.ToArray();
                GetWindow(typeof(BuildPlayerWindow));
            }
        }

        private void GenerateToggleSelectedScenes(SceneData sceneData)
        {
            SelectedScene currentSceneData = _selectedScenes.FirstOrDefault(selectedScene => selectedScene.SceneData.Path.Equals(sceneData.Path));

            if (currentSceneData == null)
            {
                return;
            }

            EditorGUILayout.BeginVertical();
            currentSceneData.IsSelected = EditorGUILayout.ToggleLeft(sceneData.Name, currentSceneData.IsSelected);
            EditorGUILayout.EndVertical();
        }

        private void DeleteRepeated()
        {
            _selectedScenes = _selectedScenes
                .GroupBy(x => x.SceneData.Path)
                .Select(x => x.First())
                .ToList();
        }

        private void ShowStandaloneMessage()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox(Standalone64WarningMsg, MessageType.Warning);
            EditorGUILayout.EndHorizontal();
        }
    }
}