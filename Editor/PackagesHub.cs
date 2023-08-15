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
        public const int DoTween = 10001;

#region Tsukat packages
        // internal use
        // public const int ServiceLocator = 0;
        // public const int LiteNet = 1;
        public const int SceneParameters = 2;
        public const int BrokenElements = 3;

#endregion
    }

    public static class PackagesHub
    {
#region Unity Menu Path

        private const string AssetUsageDetectorMenuPath = "Tsukat/Add.../Asset Usage Detector";
        private const string DoTweenMenuPath = "Tsukat/Add.../DoTween";

        private const string ServiceLocatorMenuPath = "Tsukat/Add.../Service Locator";
        private const string LiteNetMenuPath = "Tsukat/Add.../Lite Net";
        private const string SceneParametersMenuPath = "Tsukat/Add.../Scene Parameters";
        private const string BrokenElementMenuPath = "Tsukat/Add.../Broken Elements";

#endregion

        private const string AssetUsageDetectorPackagePath = "https://github.com/yasirkula/UnityAssetUsageDetector.git";
        private const string DoTweenPackagePath = "https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676";
        
		// internal use
        // private const string ServiceLocatorPackagePath = "git+https://tsukat-studio@dev.azure.com/tsukat-studio/TSUKAT%20Development/_git/TsukatUnityPackages#ServiceLocator";
        // private const string LiteNetPackagePath = "git+https://tsukat-studio@dev.azure.com/tsukat-studio/TSUKAT%20Development/_git/TsukatUnityPackages#LiteNet";
        // private const string BrokenElementPackagePath = "git+https://tsukat-studio@dev.azure.com/tsukat-studio/TSUKAT%20Development/_git/TsukatUnityPackages#TestBrokenElements";
        // private const string SceneParametersPackagePath = "git+https://tsukat-studio@dev.azure.com/tsukat-studio/TSUKAT%20Development/_git/TsukatUnityPackages#SceneParameters";
        
        private const string BrokenElementPackagePath = "https://github.com/FidgHorlov/SeparateTools.git#missingElements";
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
        
        [MenuItem(DoTweenMenuPath, false, MenuPriority.DoTween)]
        private static void AddDoTween()
        {
            Application.OpenURL(DoTweenPackagePath);
        }

		// internal use
        // [MenuItem(ServiceLocatorMenuPath, false, MenuPriority.ServiceLocator)]
        // private static void AddServiceLocator()
        // {
        //     AddPackage(ServiceLocatorPackagePath);
        // }
        //
        // [MenuItem(LiteNetMenuPath, false, MenuPriority.LiteNet)]
        // private static void AddLiteNet()
        // {
        //     AddPackage(LiteNetPackagePath);
        // }
        
        [MenuItem(BrokenElementMenuPath, false, MenuPriority.BrokenElements)]
        private static void AddBrokenElements()
        {
            AddPackage(BrokenElementPackagePath);
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