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
using UnityEngine.SceneManagement;

namespace TsukatTool.Editor.SceneParameters
{
    public class SceneParametersMainWindow : EditorWindow
    {
        private const string ScenesHeader =
            "Here you can select which scenes you want to see on the Menu tabs.\r\nAlso, if you have to build your project for differents platform you could manage it here.";

        private const string ApplySettingsButtonName = "Apply";
        private const string OpenSettingsButtonName = "Settings";
        private const string PrepareBuildButtonName = "Prepare build";

        private const string TargetPlatformHeader = "Target Platform";
        private const string AddToMenuLabel = "Add to menu?";
        private const string AddToBuildLabel = "Add to build";

        private const string AddBuildFromSettingsLabel = "Add build targets from settings";
        private const string SettingChangedPrefsName = "IsSettingChanged";

        private TargetPlatformSettings _targetPlatformSettings;
        private ScenesSettings _scenesSettings;
        private Vector2 _scrollPosition = new Vector2();

        private bool _wasDictionaryInit;
        private bool _isSceneThings;
        private bool _isMoreDeepSceneEdit;
        private bool _isUnsupportedPlatformsVisible;

        private void OnGUI()
        {
            SceneUpdate();
            OpenPrepareBuild();
        }

        private void OnFocus()
        {
            UpdateSettingsIfNeeded();
        }

        private void OnBecameVisible()
        {
            UpdateSettingsIfNeeded();
        }

        private void UpdateSettingsIfNeeded()
        {
            if (!EditorPrefs.GetBool(SettingChangedPrefsName))
            {
                return;
            }

            _wasDictionaryInit = false;
            EditorPrefs.SetBool(SettingChangedPrefsName, false);
        }

        private void CreateButton(string buttonName, Action callback)
        {
            if (GUILayout.Button(buttonName))
            {
                callback?.Invoke();
            }
        }

        private void SceneUpdate()
        {
            EditorGUILayout.LabelField(ScenesHeader, EditorStyles.wordWrappedLabel);
            if (!_wasDictionaryInit)
            {
                InitSettings();
                _wasDictionaryInit = true;
            }

            ScrollWithAllScenes();

            EditorGUILayout.BeginHorizontal(EditorStyles.inspectorFullWidthMargins);
            CreateButton(ApplySettingsButtonName, AddAllScenesToBuildWindow);
            CreateButton(OpenSettingsButtonName, SceneParametersRunner.OpenSettingsWindow);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
        }

        private void ScrollWithAllScenes()
        {
            EditorGUILayout.BeginVertical(
                EditorStyles.helpBox,
                GUILayout.ExpandHeight(false),
                GUILayout.ExpandWidth(true));

            _scrollPosition =
                GUILayout.BeginScrollView(_scrollPosition,
                    false,
                    false,
                    GUILayout.ExpandHeight(false));

            foreach (SceneData scene in _scenesSettings.ScenesData)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField(scene.Name, EditorStyles.boldLabel);
                scene.IsBuildAdded = EditorGUILayout.ToggleLeft(AddToBuildLabel, scene.IsBuildAdded);
                scene.IsCustomSceneLoader = EditorGUILayout.ToggleLeft(AddToMenuLabel, scene.IsCustomSceneLoader);
                if (_targetPlatformSettings?.BuildTargets == null || _targetPlatformSettings.BuildTargets.Length < 1)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.LabelField(AddBuildFromSettingsLabel);
                    EditorGUILayout.EndVertical();
                    continue;
                }

                if (!scene.IsBuildAdded)
                {
                    continue;
                }

                scene.IsBuildTargetSelected = EditorGUILayout.BeginFoldoutHeaderGroup(scene.IsBuildTargetSelected, TargetPlatformHeader);

                if (scene.IsBuildTargetSelected)
                {
                    TargetBuildSelection(scene);
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            if (IsUnsupportedPlatformSelected())
            {
                string prefix = _isUnsupportedPlatformsVisible ? "Hide" : "Show";
                
                if (GUILayout.Button(prefix + " unsupported platforms", EditorStyles.miniButton))
                {
                    _isUnsupportedPlatformsVisible = !_isUnsupportedPlatformsVisible;
                }
            }

            GUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void TargetBuildSelection(SceneData sceneData)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            foreach (CustomBuildTarget buildTarget in _targetPlatformSettings.BuildTargets)
            {
                if (buildTarget.Name.Equals(BuildTarget.NoTarget.ToString()))
                {
                    EditorGUILayout.LabelField(AddBuildFromSettingsLabel);
                    continue;
                }

                if (!buildTarget.IsSelected)
                {
                    Debug.Log($"[{sceneData.Name}]. Build target {buildTarget.Name} not selected.");
                    continue;
                }

                CustomBuildTarget selectedTarget = GetBuildTarget(sceneData, buildTarget.Name);
                if (selectedTarget == null)
                {
                    continue;
                }

                if (!selectedTarget.ScenePath.Equals(sceneData.Path) && !string.IsNullOrEmpty(selectedTarget.ScenePath))
                {
                    Debug.Log($"[{sceneData.Name}]. Scene path not equal ({sceneData.Path} and {selectedTarget.ScenePath})  Build target - {selectedTarget.Name}");
                    continue;
                }

                selectedTarget.IsSelected = EditorGUILayout.ToggleLeft(selectedTarget.Name, selectedTarget.IsSelected);
            }
            
            if (_isUnsupportedPlatformsVisible)
            {
                ShowUnsupportedBuildTargets(sceneData);
            }

            EditorGUILayout.EndVertical();
        }

        private bool IsUnsupportedPlatformSelected() => _targetPlatformSettings.BuildTargets.Any(buildTarget => buildTarget.IsUnsupported && buildTarget.IsSelected);

        private void ShowUnsupportedBuildTargets(SceneData sceneData)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (!sceneData.IsBuildAdded)
            {
                return;
            }
            
            EditorGUILayout.LabelField("Unsupported and selected", EditorStyles.boldLabel);

            foreach (CustomBuildTarget customBuildTarget in _targetPlatformSettings.BuildTargets)
            {
                if (!customBuildTarget.IsSelected || !customBuildTarget.IsUnsupported)
                {
                    continue;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(customBuildTarget.Name, EditorStyles.miniLabel);
                if (GUILayout.Button("Add"))
                {
                    bool isParsed = Enum.TryParse(customBuildTarget.Name, out BuildTargetGroup buildTargetGroup);
                    if (isParsed)
                    {
                        EditorUserBuildSettings.selectedBuildTargetGroup = buildTargetGroup;
                    }

                    GetWindow(typeof(BuildPlayerWindow));
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        private CustomBuildTarget GetBuildTarget(SceneData sceneData, string settingName)
        {
            return sceneData.TargetPlatformSettings.BuildTargets.FirstOrDefault(customBuildTarget => customBuildTarget.Name.Equals(settingName));
        }

        private void InitSettings()
        {
            _targetPlatformSettings = FileManager.LoadTargetPlatforms();
            _scenesSettings = FileManager.LoadScenesSettings();

            if (_targetPlatformSettings == null)
            {
                _targetPlatformSettings = new TargetPlatformSettings {BuildTargets = new CustomBuildTarget[1]};
                CustomBuildTarget customBuildTarget = new CustomBuildTarget
                {
                    Name = BuildTarget.NoTarget.ToString(),
                    ScenePath = string.Empty,
                    IsSelected = false
                };
                _targetPlatformSettings.BuildTargets = new[]
                {
                    customBuildTarget
                };
            }

            List<SceneData> sceneDataList = new List<SceneData>();
            bool isBuildSettingsWithoutPath = _scenesSettings == null || _scenesSettings.ScenesData.Length == 0;
            foreach (Scene scene in ScenesGetter.OpenSceneOneByOne())
            {
                SceneData sceneData = isBuildSettingsWithoutPath ? CreateNewSceneData(scene) : GetSceneDataIfExist(scene);
                sceneData.IsBuildAdded = SceneUtility.GetBuildIndexByScenePath(scene.path) > -1;
                sceneData.IsCustomSceneLoader = SceneLoader.SceneLoader.IsSceneInMenu(scene.name);
                sceneDataList.Add(sceneData);
            }

            _scenesSettings = new ScenesSettings {ScenesData = sceneDataList.ToArray()};
        }

        private SceneData CreateNewSceneData(Scene scene)
        {
            SceneData sceneData = new SceneData
            {
                Scene = scene,
                Name = scene.name,
                Path = scene.path,
                TargetPlatformSettings = new TargetPlatformSettings()
            };

            if (_targetPlatformSettings == null)
            {
                return sceneData;
            }

            sceneData.TargetPlatformSettings.BuildTargets = new CustomBuildTarget[_targetPlatformSettings.BuildTargets.Length];
            for (int index = 0; index < _targetPlatformSettings.BuildTargets.Length; index++)
            {
                CustomBuildTarget buildTarget = new CustomBuildTarget
                {
                    Name = _targetPlatformSettings.BuildTargets[index].Name,
                    IsSelected = false,
                    ScenePath = sceneData.Path
                };
                sceneData.TargetPlatformSettings.BuildTargets[index] = buildTarget;
            }

            return sceneData;
        }

        private void OpenPrepareBuild()
        {
            EditorGUILayout.BeginVertical();
            CreateButton(PrepareBuildButtonName, SceneParametersRunner.OpenPrepareBuildWindow);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
        }

        private void AddAllScenesToBuildWindow()
        {
            List<EditorBuildSettingsScene> settingsScenes = new List<EditorBuildSettingsScene>();
            foreach (SceneData scene in _scenesSettings.ScenesData)
            {
                if (scene.IsBuildAdded)
                {
                    settingsScenes.Add(new EditorBuildSettingsScene(scene.Path, true));
                }
            }

            EditorBuildSettings.scenes = settingsScenes.ToArray();
            SceneLoader.SceneLoader.AddAllScenesToMenuBar(_scenesSettings.ScenesData);
            FileManager.ReWriteScenesSettings(_scenesSettings);
        }

        private SceneData GetSceneDataIfExist(Scene scene) => _scenesSettings.ScenesData.First(sceneData => sceneData.Path.Equals(scene.path));
    }
}