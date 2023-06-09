#region Info

// Tsukat tool - by Horlov Andrii (andreygorlovv@gmail.com)
// Tsukat -> https://tsukat.com/

#endregion

using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace TsukatTool.Editor
{
    public static class PackagesHub
    {
#region Unity Menu Path

        private const string AssetUsageDetectorMenuPath = "Tsukat/Add.../Asset Usage Detector";
        private const string MissingElementMenuPath = "Tsukat/Add.../Missing Elements";

#endregion

        private const string AssetUsageDetectorName = "com.yasirkula.assetusagedetector";
        private const string MissingElementPackagePath = "https://github.com/FidgHorlov/SeparateTools.git#missingElements";

        private const string PackageInstalledMsg = "Package installed!";

        private static ListRequest _request;
        private static string _currentPackageName;

        [MenuItem(AssetUsageDetectorMenuPath, false, 10000)]
        private static void AddUsageDetector()
        {
            AddPackage(AssetUsageDetectorName);
        }
        
        [MenuItem(MissingElementMenuPath, false)]
        private static void AddMissingElement()
        {
            AddPackage(MissingElementPackagePath);
        }

        private static void AddPackage(string path)
        {
            _request = Client.List(offlineMode: false);
            EditorApplication.update += Progress;
            _currentPackageName = path;
        }

        private static void Progress()
        {
            if (!_request.IsCompleted)
            {
                return;
            }

            switch (_request.Status)
            {
                case StatusCode.Success:
                    if (IsPackageAlreadyInstalled())
                    {
                        return;
                    }

                    EditorApplication.update += AddPackageProgress;
                    break;
                case StatusCode.Failure:
                    Debug.LogError(_request.Error.message);
                    break;
            }

            EditorApplication.update -= Progress;
        }

        private static bool IsPackageAlreadyInstalled()
        {
            foreach (PackageInfo packageInfo in _request.Result)
            {
                if (!packageInfo.name.Equals(_currentPackageName))
                {
                    continue;
                }

                Debug.Log($"<b>{_currentPackageName}</b> is already Installed!");
                EditorApplication.update -= Progress;
                return true;
            }

            return false;
        }

        private static void AddPackageProgress()
        {
            if (!_request.IsCompleted)
            {
                return;
            }

            switch (_request.Status)
            {
                case StatusCode.Success:
                    Debug.Log(PackageInstalledMsg);
                    break;
                case StatusCode.Failure:
                    Debug.LogError(_request.Error.message);
                    break;
            }

            EditorApplication.update -= AddPackageProgress;
        }
    }
}