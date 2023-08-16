using System.IO;
using UnityEngine;

namespace Logger
{
    public static class Constants
    {
        private const string LogFolder = "Logs";
        private static string _folderPath;

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