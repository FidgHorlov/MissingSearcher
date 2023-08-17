using System;
using System.IO;
using Logger.Utilities;
using UnityEngine;

namespace Logger
{
    public class Log
    {
        private const string BigLogStartWarning = "[Logger information]. To find a new log entrance, please, search for -> \"###NEW_LOG\"\r\n";
        private const string StackTraceStartSeparator = "\r\n-----------------CODE PATH:\r\n";
        private const string EndLogMessageSeparator = "-----------------END LOG MESSAGE!\r\n\n\n";
        private const string BigLogSeparator = "\r\n###NEW_LOG\r\n";

        private static readonly LogSettingsModel LOGSettings;
        private static string _filePath;
        private static bool _wasInit;

        static Log()
        {
            if (_wasInit)
            {
                return;
            }
            
            LOGSettings = FileManager.LoadSettings();
            if (LOGSettings.IsLogActive)
            {
                CreateLogFile();
            }

            _wasInit = true;
            Application.logMessageReceivedThreaded += OnLogMessageReceived;
        }

        ~Log()
        {
            Application.logMessageReceivedThreaded -= OnLogMessageReceived;
        }

        /// <summary>
        /// Logger init.
        /// All start actions in Constructor.
        /// </summary>
        internal static void LoggerInit()
        {
        }

        /// <summary>
        /// Called for every message logged in the Unity console
        /// <para>NOTE: Might be called in a different Thread so this method has to be thread-safe!</para>
        /// </summary>
        /// <param name="logString"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        private static void OnLogMessageReceived(string logString, string stackTrace, LogType type)
        {
            if (!LOGSettings.IsLogActive)
            {
                return;
            }

            DateTime time = DateTime.Now;
            string timeStamp = $"{time.Hour:D2}:{time.Minute:D2}:{time.Second:D2}.{time.Millisecond:D3}";
            string fullMessage = $"### {timeStamp}. LOG TYPE: {type}\r\n{logString}\r\n";
            if (LOGSettings.IsFullLogs)
            {
                fullMessage += StackTraceStartSeparator + stackTrace + EndLogMessageSeparator;
            }
            else
            {
                fullMessage += "\r\n";
            }

            WriteToFile(fullMessage);
        }

        /// <summary>
        /// Create log file depend on settings. 
        /// </summary>
        private static void CreateLogFile()
        {
            FileManager.CreateFolderIfNeeded(GetFolderPath());
            _filePath = GetFilePath();
            Debug.Log($"Exporting Log File to {_filePath}");
            switch (LOGSettings.LogFileType)
            {
                case LogFileType.OneBigFile:
                    if (!File.Exists(_filePath))
                    {
                        FileManager.AddTextToFile(_filePath, BigLogStartWarning);
                    }
                    WriteToFile(BigLogSeparator);
                    return;
                case LogFileType.SeparateFiles:
                    DeleteLogsIfMore();
                    break;
            }

            FileManager.DeleteFile(_filePath); // delete file in case of duplicate
        }

        /// <summary>
        /// Delete files if we have more than user set in Settings.
        /// </summary>
        private static void DeleteLogsIfMore()
        {
            if (LOGSettings.MaxLogFiles == -1)
            {
                return;
            }

            FileManager.DeleteFiles(GetFolderPath(), LogPaths.LogSearchPattern, LOGSettings.MaxLogFiles);
        }

        /// <summary>
        /// Get file path depends on log type.
        /// </summary>
        /// <returns>If LogType is Separate - name will be with date.<br/>Otherwise name will be default</returns>
        private static string GetFilePath()
        {
            return Path.Combine(GetFolderPath(), LOGSettings.LogFileType == LogFileType.SeparateFiles
                ? string.Format(LogPaths.MultipleLogsFileName, DateTime.Now.ToString(LogPaths.DateTimeFormat))
                : LogPaths.OneLogFileName);
        }

        /// <summary>
        /// Write log in file if it's active
        /// </summary>
        /// <param name="message"></param>
        private static void WriteToFile(string message)
        {
            if (!LOGSettings.IsLogActive)
            {
                return;
            }

            if (!File.Exists(_filePath) && LOGSettings.LogFileType == LogFileType.OneBigFile)
            {
                FileManager.AddTextToFile(_filePath, BigLogStartWarning);
            }
            
            FileManager.AddTextToFile(_filePath, message);
        }

        /// <summary>
        /// Return Folder path depends on the settings
        /// </summary>
        /// <returns>Default path or path from settings</returns>
        private static string GetFolderPath()
        {
            string folderName = string.IsNullOrEmpty(LOGSettings.LogFolderName) ? LogPaths.LogFolder : LOGSettings.LogFolderName;
            return LogPaths.GetFolderPath(folderName);
            ;
        }
    }
}