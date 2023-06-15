using System;

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
        
        private const string MultiToolSettingsPath = "Settings/TargetPlatformSettings.settings";
        private const string ScenesSettings = "Settings/ScenesSettings.settings";

        private const string SceneLoaderTemplatePath = "SceneLoader/SceneLoaderTemplate";
        private const string MenuItemTemplatePath = "SceneLoader/MenuItemTemplate";

        private const string ReleasedFolderPath = "Packages/com.tsukat.multitool/Editor";
        private const string WorkingFolderPath = "Assets/Editor/";
        
        private static bool _isRelease = true; //todo: set TRUE when it should be used as Package

        internal static string GetPath(PathType pathType)
        {
            string path = _isRelease ? ReleasedFolderPath : WorkingFolderPath;
            
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
    }
}