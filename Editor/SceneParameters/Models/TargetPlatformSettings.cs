using System;

namespace TsukatTool.Editor.SceneParameters.Models
{
    [Serializable]
    public class TargetPlatformSettings
    {
        public CustomBuildTarget[] BuildTargets;
    }

    [Serializable]
    public class CustomBuildTarget
    {
        public string ScenePath;
        public string Name;
        public bool IsSelected;
    }
}