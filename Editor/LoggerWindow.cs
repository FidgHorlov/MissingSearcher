using System;
using Logger.Utilities;
using UnityEditor;
using UnityEngine;

namespace Logger.Editor
{
    public class LoggerWindow : EditorWindow
    {
        private enum LogCapacity
        {
            Full,
            Short
        }

        private const string LogsSearchPattern = "*.txt";

        private LogCapacity _logCapacity;
        private LogFileType _logFileType;

        private LogSettingsModel _logSettings;
        
        [MenuItem("Tsukat/Loger configuration")]
        private static void ShowLoggerConfiguration()
        {
            LoggerWindow wnd = GetWindow<LoggerWindow>();
            wnd.titleContent = new GUIContent("Logger configuration");
            wnd.Show();
        }
        
        private void OnGUI()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Not working in Play mode.\r\nPlease, configure login in Editor Mode", MessageType.Warning);
                return;
            }
            
            EditorGUILayout.HelpBox("Here you can setup the logger", MessageType.Info);
            if (_logSettings == null)
            {
                _logSettings = FileManager.LoadSettings();
            }

            _logSettings.IsLogActive = EditorGUILayout.Toggle("Activate Log system?", _logSettings.IsLogActive); 
            if (!_logSettings.IsLogActive)
            {
                SaveButtonPressed();
                DeleteButtonPressed();
                ShowLogsPath();
                return;
            }

            LogCapacityGui();
            LogFileWriting();
            if (_logSettings.LogFileType == LogFileType.SeparateFiles)
            {
                CountLogFiles();
            }

            SaveButtonPressed();
            DeleteButtonPressed();
            ShowLogsPath();
        }

        private void ShowLogsPath()
        {
            _logSettings.LogFolderPath = Constants.FolderPath;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Logs folder", EditorStyles.boldLabel);
            GUILayout.TextArea(_logSettings.LogFolderPath, EditorStyles.miniLabel);
            if (GUILayout.Button("Open folder"))
            {
                Application.OpenURL(_logSettings.LogFolderPath);
            }
            EditorGUILayout.EndVertical();
        }

        private void DeleteButtonPressed()
        {
            if (GUILayout.Button("Delete logs"))
            {
                FileManager.DeleteFiles(_logSettings.LogFolderPath, LogsSearchPattern);
            }
        }

        private void SaveButtonPressed()
        {
            if (GUILayout.Button("Save"))
            {
                SaveSettings();
            }
        }

        private void CountLogFiles()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("How much files you wanna save?");
            _logSettings.MaxLogFiles = EditorGUILayout.IntField("Count:", _logSettings.MaxLogFiles);
            EditorGUILayout.LabelField("Write -1, if you want to keep all files", EditorStyles.miniLabel);
        }

        private void SaveSettings()
        {
            FileManager.SaveSettings(_logSettings);
        }

        private void LogFileWriting()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("File writing", EditorStyles.label);
            _logSettings.LogFileType = (LogFileType) EditorGUILayout.EnumPopup(_logSettings.LogFileType);
            EditorGUILayout.LabelField("Determinate if you need just one log file.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.EndVertical();
        }

        private void LogCapacityGui()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Log capacity", EditorStyles.label);
            _logCapacity = (LogCapacity) EditorGUILayout.EnumPopup(_logCapacity);
            _logSettings.IsFullLogs = _logCapacity switch
            {
                LogCapacity.Full => true,
                LogCapacity.Short => false,
                _ => throw new ArgumentOutOfRangeException()
            };

            EditorGUILayout.LabelField("Use Long in case, you need to see from where the log is called.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.EndVertical();
        }
    }
}