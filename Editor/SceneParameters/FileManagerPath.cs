using System;
using System.IO;
using UnityEditor;

namespace TsukatTool.Editor
{
    internal enum PathType
    {
        Settings,
        SceneSettings,
        SceneLoaderTemplate,
        MenuItemTemplate
    }

    internal static class FileManagerPath
    {
        public const string SceneLoaderTargetPath = "Assets/Tsukat/MultiTool/Editor/CustomSceneLoader.cs";

        private const string MultiToolSettingsPath = "/Settings/TargetPlatformSettings.settings";
        private const string ScenesSettings = "/Settings/ScenesSettings.settings";

        private const string SceneLoaderTemplatePath = "/SceneLoader/SceneLoaderTemplate";
        private const string MenuItemTemplatePath = "/SceneLoader/MenuItemTemplate";

        internal static string GetPath(PathType pathType)
        {
            string path = Path.GetDirectoryName(GetFilePath());
            
            switch (pathType)
            {
                case PathType.Settings:
                {
                    path += MultiToolSettingsPath;
                    break;
                }
                case PathType.SceneSettings:
                {
                    path += ScenesSettings;
                    break;
                }
                case PathType.SceneLoaderTemplate:
                {
                    path += SceneLoaderTemplatePath;
                    break;
                }
                case PathType.MenuItemTemplate:
                {
                    path += MenuItemTemplatePath;
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(pathType), pathType, null);
            }

            return path;
        }

        private static string GetFilePath()
        {
            string[] fileGui = AssetDatabase.FindAssets($"t:Script {nameof(FileManagerPath)}");
            return AssetDatabase.GUIDToAssetPath(fileGui[0]);
        }
    }
}