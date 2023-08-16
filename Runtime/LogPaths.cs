using System.IO;
using UnityEngine;

namespace Logger
{
    public static class LogPaths
    {
        private const string LogFolder = "Logs";
        private static string _folderPath;
        
        public const string MultipleLogsFileName = "DebugLogExport_{0}.txt";
        public const string OneLogFileName = "DebugLogExport.txt";
        public const string DateTimeFormat = "MM-dd-yy_hh-mm-ss";
        public const string LogSearchPattern = "*.txt";

        /// <summary>
        /// Logs Folder path.
        /// </summary>
        public static string FolderPath
        {
            get
            {
                if (string.IsNullOrEmpty(_folderPath))
                {
                    _folderPath = Path.Combine(Application.persistentDataPath, LogFolder);
                }

                return _folderPath;
            }
        }
    }
}