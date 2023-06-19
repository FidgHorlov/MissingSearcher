#region Info

// Tsukat tool - by Horlov Andrii (andreygorlovv@gmail.com)
// Tsukat -> https://tsukat.com/

#endregion

using System.IO;
using TsukatTool.Editor.SceneParameters.Models;
using UnityEditor;
using UnityEngine;

namespace TsukatTool.Editor.SceneParameters.Utilities
{
    public static class FileManager
    {
        private const string GlobalTemplateMessage = "Can't find Global Template file\r\nPath: {0}";
        private const string MenuItemTemplateMessage = "Can't find MenuItem Template file\r\nPath: {0}";

        public static TargetPlatformSettings LoadTargetPlatforms()
        {
            string json = LoadFromFile(FileManagerPath.GetPath(PathType.Settings));
            return JsonUtility.FromJson<TargetPlatformSettings>(json);
        }

        public static void ReWriteTargetPlatforms(TargetPlatformSettings targetPlatformSettings)
        {
            string jsonData = JsonUtility.ToJson(targetPlatformSettings, true);
            ReWriteFile(FileManagerPath.GetPath(PathType.Settings), jsonData);
        }

        public static ScenesSettings LoadScenesSettings()
        {
            string json = LoadFromFile(FileManagerPath.GetPath(PathType.SceneSettings));
            return JsonUtility.FromJson<ScenesSettings>(json);
        }

        public static void ReWriteScenesSettings(ScenesSettings scenesSettings)
        {
            string jsonData = JsonUtility.ToJson(scenesSettings, true);
            ReWriteFile(FileManagerPath.GetPath(PathType.SceneSettings), jsonData);
        }

        public static void ReWriteSceneLoader(string data)
        {
            ReWriteFile(FileManagerPath.SceneLoaderTargetPath, data);
        }

        public static string LoadSceneLoader()
        {
            return LoadFromFile(FileManagerPath.SceneLoaderTargetPath);
        }

        public static string GetSceneLoaderTemplate()
        {
            string sceneLoaderTargetPath = FileManagerPath.SceneLoaderTargetPath;

            string fileText = LoadFromFile(sceneLoaderTargetPath);
            if (!string.IsNullOrEmpty(fileText))
            {
                return fileText;
            }

            string templateDataPath = FileManagerPath.GetPath(PathType.SceneLoaderTemplate);
            StreamReader templateStream = File.OpenText(templateDataPath);
            ReWriteSceneLoader(templateStream.ReadToEnd());
            templateStream.Close();

            fileText = LoadFromFile(sceneLoaderTargetPath);
            if (!string.IsNullOrEmpty(fileText))
            {
                return fileText;
            }

            Debug.Log($"Can't find SceneLoaderTemplate\r\non this path:{FileManagerPath.SceneLoaderTargetPath}");
            return null;
        }

        public static string GetMenuItemTemplate()
        {
            return LoadFromFile(FileManagerPath.GetPath(PathType.MenuItemTemplate));
        }

        public static bool CheckIfTemplatesExist()
        {
            if (!File.Exists(FileManagerPath.GetPath(PathType.SceneLoaderTemplate)))
            {
                Debug.LogError(string.Format(GlobalTemplateMessage, FileManagerPath.GetPath(PathType.SceneLoaderTemplate)));
                return false;
            }

            if (File.Exists(FileManagerPath.GetPath(PathType.MenuItemTemplate)))
            {
                return true;
            }

            Debug.LogError(string.Format(MenuItemTemplateMessage, FileManagerPath.GetPath(PathType.MenuItemTemplate)));
            return false;
        }

        private static void ReWriteFile(string pathFile, string jsonData)
        {
            if (!File.Exists(pathFile))
            {
                string folderPath = Path.GetDirectoryName(pathFile);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                FileStream fileStream = File.Create(pathFile);
                fileStream.Close();
                fileStream.Dispose();
            }

            File.WriteAllText(pathFile, string.Empty);
            StreamWriter streamWriter = new StreamWriter(pathFile);
            streamWriter.Write(jsonData);
            streamWriter.Close();
            AssetDatabase.Refresh();
        }


        private static string LoadFromFile(string pathFile)
        {
            if (!File.Exists(pathFile))
            {
                return null;
            }

            StreamReader streamReader = File.OpenText(pathFile);
            string fileData = streamReader.ReadToEnd();
            streamReader.Close();
            return fileData;
        }
    }
}