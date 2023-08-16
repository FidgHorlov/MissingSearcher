using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Logger.Utilities
{
    public static class FileManager
    {
        private const string SettingsPath = "/LoggerSettings.settings";

        public static LogSettingsModel LoadSettings()
        {
            string jsonSettings = LoadJsonFile();
            return jsonSettings == null ? CreateDefaultSettings() : GetSettingModel(jsonSettings);
        }

        public static void SaveSettings(LogSettingsModel logSettings)
        {
            string jsonSettings = GetSettingsJson(logSettings);
            ReWriteFile(GetSettingsFileFolder() + SettingsPath, jsonSettings);
        }

        /// <summary>
        /// Create folder if exist.
        /// </summary>
        /// <param name="folderPath">Path for folder (can be null)</param>
        /// <returns>True - if folder already exist. That means, maybe there duplicate of files.</returns>
        public static bool CreateFolderIfNeeded(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return false;
            }

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            else
            {
                return true;
            }

            return false;
        }

        public static async void DeleteFiles(string folderPath, string searchPattern, int maxFiles = -1)
        {
            DirectoryInfo logsDirectory = new DirectoryInfo(folderPath);
            FileInfo[] filesPaths = logsDirectory.GetFiles(searchPattern);
            if (maxFiles != -1)
            {
                if (maxFiles > filesPaths.Length)
                {
                    return;
                }
            }

            filesPaths = filesPaths.OrderByDescending(file => file.CreationTimeUtc).ToArray();
            // await Task.Run(() => File.Delete(filesPaths[^1].FullName));
            await Task.Run(() => DeleteFile(filesPaths[^1].FullName));
        }

        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public static void AddTextToFile(string filePath, string message)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                File.AppendAllText(filePath, message);
            }
        }

        private static LogSettingsModel CreateDefaultSettings()
        {
            LogSettingsModel logSettingsModel = new LogSettingsModel
            {
                IsLogActive = false,
                IsFullLogs = false,
                LogFileType = LogFileType.OneFile,
                MaxLogFiles = -1,
                LogFolderPath = Constants.FolderPath
            };

            return logSettingsModel;
        }

        private static string LoadJsonFile()
        {
            string settings = LoadFromFile(GetSettingsFileFolder() + SettingsPath);
            return string.IsNullOrEmpty(settings) ? null : settings;
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
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
            Debug.Log($"Logger settings saved!");
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

        private static LogSettingsModel GetSettingModel(string jsonSettings) => JsonUtility.FromJson<LogSettingsModel>(jsonSettings);

        private static string GetSettingsJson(LogSettingsModel logSettings) => JsonUtility.ToJson(logSettings, true);

        private static string GetSettingsFileFolder()
        {
#if UNITY_EDITOR
            return GetSettingsFileFolderUnity();
#endif
            return GetSettingsFileFolderBuild();
        }

        private static string GetSettingsFileFolderUnity()
        {
            string[] fileGui = AssetDatabase.FindAssets($"t:Script {nameof(FileManager)}");
            string path = AssetDatabase.GUIDToAssetPath(fileGui[0]);
            path = Directory.GetParent(path)?.ToString();
            return Path.GetDirectoryName(path);
        }

        private static string GetSettingsFileFolderBuild()
        {
            return Application.persistentDataPath;
        }
    }
}