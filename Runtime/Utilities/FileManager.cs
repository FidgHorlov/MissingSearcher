using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Logger.Utilities
{
    public static class FileManager
    {
        private const string SettingsPath = "/Resources/LoggerSettings.txt";
        private const string FileName = "LoggerSettings";

        /// <summary>
        /// Load settings from Json file.
        /// </summary>
        /// <returns>LogSetting</returns>
        public static LogSettingsModel LoadSettings()
        {
            string jsonFile = LoadJsonFile();
            return jsonFile == null ? CreateDefaultSettings() : GetSettingModel(jsonFile);
        }

        /// <summary>
        /// Save Log in Json file.
        /// </summary>
        /// <param name="logSettings">LogSettings</param>
        public static void SaveSettings(LogSettingsModel logSettings) => ReWriteFile(Application.dataPath + SettingsPath, GetSettingsJson(logSettings));

        /// <summary>
        /// Create folder if exist.
        /// </summary>
        /// <param name="folderPath">Path for folder (can be null)</param>
        /// <returns>True - if folder already exist. That means, maybe there duplicate of files.</returns>
        public static bool CreateFolderIfNeeded(string folderPath)
        {
            Debug.Log($"Trying to create folder -> {folderPath}");
            if (string.IsNullOrEmpty(folderPath))
            {
                return false;
            }

            if (!Directory.Exists(folderPath))
            {
                Debug.Log($"Trying to create folder. -> {folderPath}");
                Directory.CreateDirectory(folderPath);
            }
            else
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Delete all files with next parameters
        /// </summary>
        /// <param name="folderPath">Path of target folder</param>
        /// <param name="searchPattern">Search pattern of target file (type "*.extension" - if you want to delete all files with target "extension") </param>
        /// <param name="maxFiles">Mostly used if we have limitation for files amount</param>
        public static async void DeleteFiles(string folderPath, string searchPattern, int maxFiles = -1)
        {
            if (!Directory.Exists(folderPath))
            {
                return;
            }
            
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
            await Task.Run(() => DeleteFile(filesPaths[^1].FullName));
        }

        /// <summary>
        /// Delete target file if its exists
        /// </summary>
        /// <param name="filePath">Path to file we want delete</param>
        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// Add text to file<br/>Used for log to write log one by one.
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <param name="message">Target message</param>
        public static void AddTextToFile(string filePath, string message)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                File.AppendAllText(filePath, message);
            }
        }

        /// <summary>
        /// Create default settings if they're not exist
        /// </summary>
        /// <returns>Log Setting Model with default values</returns>
        private static LogSettingsModel CreateDefaultSettings()
        {
            LogSettingsModel logSettingsModel = new LogSettingsModel
            {
                IsLogActive = false,
                IsFullLogs = false,
                LogFileType = LogFileType.OneFile,
                MaxLogFiles = -1,
                LogFolderPath = LogPaths.FolderPath
            };

            return logSettingsModel;
        }

        /// <summary>
        /// Load Settings json file
        /// </summary>
        /// <returns>Json with settings</returns>
        private static string LoadJsonFile()
        {
            TextAsset settings = Resources.Load<TextAsset>(FileName);
            Debug.Log($"Setting file here: {settings?.text}");
            return string.IsNullOrEmpty(settings?.text) ? null : settings.ToString();
        }

        /// <summary>
        /// Rewrite target file
        /// </summary>
        /// <param name="pathFile">File we need to rewrite</param>
        /// <param name="jsonData">Data we need to replace in file</param>
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
            Debug.Log($"Logger settings saved!\r\n{pathFile}");
        }

        /// <summary>
        /// Convert Json file to Log Setting 
        /// </summary>
        /// <param name="jsonSettings">Json file with settings</param>
        /// <returns>Log Settings from Json file</returns>
        private static LogSettingsModel GetSettingModel(string jsonSettings) => JsonUtility.FromJson<LogSettingsModel>(jsonSettings);

        /// <summary>
        /// Convert Log Settings to Json file
        /// </summary>
        /// <param name="logSettings">Log Settings</param>
        /// <returns>Json file with Log Settings</returns>
        private static string GetSettingsJson(LogSettingsModel logSettings) => JsonUtility.ToJson(logSettings, true);
    }
}