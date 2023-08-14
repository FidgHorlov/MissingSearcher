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
    public class SceneParametersSettingsWindow : EditorWindow, IHasCustomMenu
    {
        private const string ButtonName = "Apply settings";
        private const string Header = "Select all build platform you will use";
        private const string HeaderNotSupportedPlatforms = "NOT SUPPORTED PLATFORMS:";
        private const string ContextMenuUnsupportedPlatforms = " Unsupported Platforms";
        private const string SettingChangedPrefsName = "IsSettingChanged";

        private TargetPlatformSettings _selectedBuildTargets;
        private List<CustomBuildTarget> _buildTargets;

        private Vector2 _scrollPosition;
        private bool _inited;
        private bool _isNotSupportedPlatformsVisible = false;

        private void OnGUI()
        {
            EditorGUILayout.LabelField(Header);
            if (!_inited)
            {
                InitBuildDictionary();
                _inited = true;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox,
                GUILayout.ExpandHeight(false),
                GUILayout.ExpandWidth(true));

            _scrollPosition = GUILayout.BeginScrollView(
                _scrollPosition,
                false,
                false,
                GUILayout.ExpandHeight(false)
            );

            foreach (CustomBuildTarget buildTarget in _buildTargets)
            {
                if (buildTarget.IsUnsupported)
                {
                    continue;
                }

                buildTarget.IsSelected = EditorGUILayout.ToggleLeft(buildTarget.Name, buildTarget.IsSelected);
            }

            if (_isNotSupportedPlatformsVisible)
            {
                EditorGUILayout.LabelField(HeaderNotSupportedPlatforms);

                foreach (CustomBuildTarget buildTarget in _buildTargets)
                {
                    if (!buildTarget.IsUnsupported)
                    {
                        continue;
                    }

                    buildTarget.IsSelected = EditorGUILayout.ToggleLeft(buildTarget.Name, buildTarget.IsSelected);
                }
            }

            GUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            if (!GUILayout.Button(ButtonName))
            {
                return;
            }

            _selectedBuildTargets = new TargetPlatformSettings
            {
                BuildTargets = GetSortedBuildTargets().ToArray()
            };

            FileManager.ReWriteTargetPlatforms(_selectedBuildTargets);
            EditorPrefs.SetBool(SettingChangedPrefsName, true);
            FocusWindowIfItsOpen<SceneParametersMainWindow>();
        }

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            string nameOfContextMenu = _isNotSupportedPlatformsVisible ? "Hide" : "Show";
            nameOfContextMenu += ContextMenuUnsupportedPlatforms;

            GUIContent unsupportedPlatformsVisibility = new GUIContent(nameOfContextMenu);
            menu.AddItem(unsupportedPlatformsVisibility, false, LockUnlock);

            GUIContent unselectAll = new GUIContent("Unselect all");
            menu.AddItem(unselectAll, false, UnselectAll);
        }

        private void InitBuildDictionary()
        {
            _buildTargets = new List<CustomBuildTarget>();

            foreach (BuildTarget build in Enum.GetValues(typeof(BuildTarget)))
            {
                CustomBuildTarget customBuildTarget = new CustomBuildTarget
                {
                    Name = build.ToString(),
                    IsSelected = false,
                    ScenePath = string.Empty,
                    IsUnsupported = !IsBuildTargetSupported(build)
                };

                _buildTargets.Add(customBuildTarget);
            }

            _selectedBuildTargets = FileManager.LoadTargetPlatforms();
            if (_selectedBuildTargets == null)
            {
                return;
            }

            foreach (CustomBuildTarget allBuildTarget in _buildTargets)
            {
                foreach (CustomBuildTarget buildTarget in _selectedBuildTargets.BuildTargets)
                {
                    if (allBuildTarget.Name.Equals(buildTarget.Name))
                    {
                        allBuildTarget.IsSelected = true;
                    }
                }
            }

            _buildTargets = new List<CustomBuildTarget>(_buildTargets.OrderByDescending(buildTarget => buildTarget.IsSelected).ToArray());
        }
        
        private void UnselectAll()
        {
            foreach (CustomBuildTarget buildTarget in _buildTargets)
            {
                buildTarget.IsSelected = false;
            }
        }

        private List<CustomBuildTarget> GetSortedBuildTargets() => _buildTargets.FindAll(buildTarget => buildTarget.IsSelected).ToList();
        private bool IsBuildTargetSupported(BuildTarget buildTarget) => BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Unknown, buildTarget);
        private void LockUnlock() => _isNotSupportedPlatformsVisible = !_isNotSupportedPlatformsVisible;
    }
}