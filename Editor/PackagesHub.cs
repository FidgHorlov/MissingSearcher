#region Info

// Tsukat tool - by Horlov Andrii (andreygorlovv@gmail.com)
// Tsukat -> https://tsukat.com/

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace TsukatTool.Editor
{
    public static class MenuPriority
    {
        public const int AssetUsageDetector = 10000;

#region Tsukat packages

        public const int SceneParameters = 0;
        public const int MissingElements = 1;

#endregion
    }

    public static class PackagesHub
    {
#region Unity Menu Path

        private const string AssetUsageDetectorMenuPath = "Tsukat/Add.../Asset Usage Detector";
        private const string MissingElementMenuPath = "Tsukat/Add.../Missing Elements";
        private const string SceneParametersMenuPath = "Tsukat/Add.../Scene Parameters";

#endregion

        private const string AssetUsageDetectorPackagePath = "https://github.com/yasirkula/UnityAssetUsageDetector.git";
        private const string MissingElementPackagePath = "https://github.com/FidgHorlov/SeparateTools.git#missingElements";
        private const string SceneParametersPackagePath = "https://github.com/FidgHorlov/SeparateTools.git#sceneParameters";

        private const string PackageInstalledMsg = "Package installed!";
        private const string PackageAlreadyInstalledMsg = "is already installed!";

        private static ListRequest _installedPackages;
        private static AddRequest _newPackage;

        [MenuItem(AssetUsageDetectorMenuPath, false, MenuPriority.AssetUsageDetector)]
        private static void AddUsageDetector()
        {
            AddPackage(AssetUsageDetectorPackagePath);
        }

        [MenuItem(MissingElementMenuPath, false, MenuPriority.MissingElements)]
        private static void AddMissingElement()
        {
            AddPackage(MissingElementPackagePath);
        }

        [MenuItem(SceneParametersMenuPath, false, MenuPriority.SceneParameters)]
        private static void AddSceneParameters()
        {
            AddPackage(SceneParametersPackagePath);
        }

        private static void AddPackage(string path)
        {
            InitInstalledPackages();
            _newPackage = Client.Add(path);
            EditorApplication.update += Progress;
        }

        private static async void InitInstalledPackages()
        {
            ListRequest pack = Client.List(offlineMode: false);
            while (!pack.IsCompleted)
            {
                await Task.Yield();
            }

            _installedPackages = pack;
        }

        private static void Progress()
        {
            if (!_newPackage.IsCompleted)
            {
                return;
            }

            switch (_newPackage.Status)
            {
                case StatusCode.Success:
                {
                    if (IsPackageAlreadyInstalled())
                    {
                        return;
                    }

                    EditorApplication.update += AddPackageProgress;
                    break;
                }
                case StatusCode.Failure:
                {
                    Debug.LogError(_newPackage.Error.message);
                    break;
                }
                case StatusCode.InProgress:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            EditorApplication.update -= Progress;
        }

        private static bool IsPackageAlreadyInstalled()
        {
            PackageInfo foundPackageInfo = _installedPackages.Result.FirstOrDefault(package => package.name.Equals(_newPackage.Result.name));
            if (foundPackageInfo != null)
            {
                Debug.Log($"<b>{_newPackage.Result.name}</b> {PackageAlreadyInstalledMsg}");
            }

            EditorApplication.update -= Progress;
            return foundPackageInfo != null;
        }

        private static void AddPackageProgress()
        {
            if (!_newPackage.IsCompleted)
            {
                return;
            }

            switch (_newPackage.Status)
            {
                case StatusCode.Success:
                    Debug.Log(PackageInstalledMsg);
                    break;
                case StatusCode.Failure:
                    Debug.LogError(_newPackage.Error.message);
                    break;
            }

            EditorApplication.update -= AddPackageProgress;
        }
    }
}