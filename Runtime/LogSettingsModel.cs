using System;

namespace Logger
{
    public enum LogFileType
    {
        OneBigFile,
        OneFile,
        SeparateFiles
    }

    [Serializable]
    public class LogSettingsModel
    {
        public LogFileType LogFileType;
        public bool IsLogActive;
        public bool IsFullLogs;
        public int MaxLogFiles;
        public string LogFolderName;
    }
}