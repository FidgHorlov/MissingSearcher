using System;
using System.IO;
using Logger.Utilities;
using UnityEngine;

namespace Logger
{
    public class Log
    {
        private const string MultipleLogsFileName = "DebugLogExport_{0}.txt";
        private const string OneLogFileName = "DebugLogExport.txt";
        private const string DateTimeFormat = "MM-dd-yy_hh-mm-ss";
        private const string LogSearchPattern = "*.txt";
        private const string StackTraceStartSeparator = "\r\n-----------------CODE PATH:\r\n";
        private const string EndLogMessageSeparator = "-----------------END LOG MESSAGE!\r\n\n\n";
        private const string BigLogSeparator = "\r\n###NEW_LOG\r\n";
        private static string BigLogStartWarning => "[Logger information]. To find a new log entrance, please, search for -> \"###NEW_LOG\"\r\n";


        private static readonly LogSettingsModel LOGSettings;

        private static string _filePath;
        private static bool _wasInit;

        private static string FolderPath => Constants.FolderPath;

        static Log()
        {
            LOGSettings = FileManager.LoadSettings();
            Application.logMessageReceivedThreaded += OnLogMessageReceived;
        }

        ~Log()
        {
            Application.logMessageReceivedThreaded -= OnLogMessageReceived;
        }

        internal static void LoggerInit()
        {
            if (_wasInit)
            {
                return;
            }

            if (LOGSettings.IsLogActive)
            {
                CreateLogFile();
            }

            _wasInit = true;
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

        private static void CreateLogFile()
        {
            _filePath = GetFilePath();
            if (FileManager.CreateFolderIfNeeded(FolderPath))
            {
                DeleteLogsIfMore();
            }

            if (LOGSettings.LogFileType == LogFileType.OneBigFile)
            {
                WriteToFile(BigLogSeparator);
            }
            else
            {
                FileManager.DeleteFile(_filePath);
            }

            Debug.Log($"Exporting Log File to {_filePath}");
            LOGSettings.LogFolderPath = FolderPath;
        }

        private static void DeleteLogsIfMore()
        {
            if (LOGSettings.MaxLogFiles == -1)
            {
                return;
            }
            
            FileManager.DeleteFiles(FolderPath, LogSearchPattern, LOGSettings.MaxLogFiles);
        }

        private static string GetFilePath()
        {
            return Path.Combine(FolderPath, LOGSettings.LogFileType == LogFileType.SeparateFiles
                ? string.Format(MultipleLogsFileName, DateTime.Now.ToString(DateTimeFormat))
                : OneLogFileName);
        }

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
            
            if (!string.IsNullOrEmpty(_filePath))
            {
                FileManager.AddTextToFile(_filePath, message);
            }
        }
    }
}