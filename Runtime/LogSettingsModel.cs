using System;

namespace Logger
{
    [Serializable]
    public class LogSettingsModel
    {
        public LogFileType LogFileType;
        public bool IsLogActive;
        public bool IsFullLogs;
        public int MaxLogFiles;
        public string LogFolderPath;
    }
    
    public enum LogFileType
    {
        OneBigFile,
        OneFile,
        SeparateFiles
    }
}