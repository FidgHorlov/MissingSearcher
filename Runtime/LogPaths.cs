using System.IO;
using UnityEngine;

namespace Logger
{
    public static class LogPaths
    {
        public const string LogFolder = "Logs";

        public const string MultipleLogsFileName = "DebugLogExport_{0}.txt";
        public const string OneLogFileName = "DebugLogExport.txt";
        public const string DateTimeFormat = "MM-dd-yy_hh-mm-ss";
        public const string LogSearchPattern = "*.txt";
        
        private static string _folderPath;
        private static string LogsRootPath => Application.persistentDataPath;

        /// <summary>
        /// Convert name to the full path
        /// </summary>
        /// <param name="folderName">The name of folder</param>
        /// <returns>Absolute path of target folder</returns>
        public static string GetFolderPath(string folderName)
        {
            if (string.IsNullOrEmpty(_folderPath))
            {
                _folderPath = Path.Combine(LogsRootPath, folderName);
            }

            return _folderPath;
        }
    }
}