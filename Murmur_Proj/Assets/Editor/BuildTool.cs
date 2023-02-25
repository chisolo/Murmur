using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Reporting;
using UnityEditor.AddressableAssets.Settings;

namespace App
{
    public class BuildTool
    {
        private static bool useAAB = false;
        private static bool export = false;


        [MenuItem("App/Build/Build")]
        public static void Build()
        {
            export = false;
            Build(EditorUserBuildSettings.activeBuildTarget);
        }

        [MenuItem("App/Build/BuildDev")]
        public static void BuildDev()
        {
            export = false;
            Build(EditorUserBuildSettings.activeBuildTarget, true);
        }

        [MenuItem("App/Build/BuildDev&Run")]
        public static void BuildDevAndRun()
        {
            export = false;
            Build(EditorUserBuildSettings.activeBuildTarget, true, true);
        }

        [MenuItem("App/Build/Android/BuildAPK")]
        public static void BuildAndroid()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                Debug.LogError("Not Android");
                return;
            }

            export = false;
            useAAB = false;
            Build(BuildTarget.Android);
        }

        [MenuItem("App/Build/Android/BuildAAB")]
        public static void BuildAAB()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                Debug.LogError("Not Android");
                return;
            }

            useAAB = true;
            export = false;
            Build(BuildTarget.Android);
            useAAB = false;
        }

        [MenuItem("App/Build/Android/Export")]
        public static void Export()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                Debug.LogError("Not Android");
                return;
            }

            //forceAAB = true;
            export = true;
            Build(BuildTarget.Android);
            export = false;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
        }

        public static void Build(BuildTarget target, bool isDev = false, bool isRun = false)
        {
            Debug.Log("start build + " + target);

            PreBuild(isDev);

            // TODO support other platform
            var path = "";
            var file = "";
            switch (target) {
                case BuildTarget.Android:
                    path = "../Build/Android";
                    file = "/build";
                    if (isDev) {
                        file += "_dev";
                    }

                    if (useAAB) {
                        file += ".aab";
                    } else {
                        file += ".apk";
                    }

                    if (export) {
                        file = "prj";
                    }

                    break;
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneWindows:
                    path = "../Build/Win";
                    file = "/app.exe";
                break;
                default:
                    throw new Exception("no target");
            }

            FileUtil.DeleteFileOrDirectory(path);
            Directory.CreateDirectory(path);
            path = path + file;

            // Set Build Setting
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = EditorBuildSettings.scenes.Where(x => x.enabled).Select(x => x.path).ToArray();
            buildPlayerOptions.locationPathName = path;
            buildPlayerOptions.target = target;
            //buildPlayerOptions.targetGroup = group;
            var options = BuildOptions.None;

            if (isDev) {
                //options |= BuildOptions.Development;
                options |= BuildOptions.Development | BuildOptions.ConnectWithProfiler;
                buildPlayerOptions.extraScriptingDefines = new  string[] {"IS_DEV"};
            } else {
                //options |= BuildOptions.CleanBuildCache;
            }

            if (isRun) {
                options |= BuildOptions.AutoRunPlayer;
            }

            #if UNITY_ANDROID
            Debug.Log("Im android");
            if (useAAB) {
                EditorUserBuildSettings.buildAppBundle = true;
            } else {
                EditorUserBuildSettings.buildAppBundle = false;
            }

            EditorUserBuildSettings.exportAsGoogleAndroidProject = export;

            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = "keystore/user.keystore";
            PlayerSettings.Android.keystorePass = "osejp0";

            PlayerSettings.Android.keyaliasName = "ose";
            PlayerSettings.Android.keyaliasPass = "osejp0";
            #endif

            buildPlayerOptions.options = options;

            Debug.Log("build begin");
            // Build Package
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.LogError("Build failed");
            }

            PostBuild(isDev);
        }

        public static void PreBuild(bool isDev)
        {
            var devAssets = AssetDatabase.LoadAssetAtPath<App.Dev.DevAssets>("Assets/DevAssets/Resources/DevAssets.asset");
            Debug.Log("devAssets " + devAssets);
            if (isDev) {
                devAssets.IngameDebugConsolePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Lemegeton/3rdParty/IngameDebugConsole/IngameDebugConsole.prefab");
            } else {
                devAssets.IngameDebugConsolePrefab = null;
            }
            EditorUtility.SetDirty(devAssets);
            //AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            AddressableAssetSettings.BuildPlayerContent();
            SetGradle();
        }

        private static void PostBuild(bool isDev)
        {
#if UNITY_ANDROID
            PlayerSettings.Android.useCustomKeystore = false;
#endif
            var devAssets = AssetDatabase.LoadAssetAtPath<App.Dev.DevAssets>("Assets/DevAssets/Resources/DevAssets.asset");
            devAssets.IngameDebugConsolePrefab = null;
            EditorUtility.SetDirty(devAssets);
            //AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            EditorPrefs.SetBool("GradleUseEmbedded", true);
        }

        public static void SetGradle()
        {
            // C:\Users\owenc\git\Ose\gradle-6.5

            var root = Application.dataPath;
            root = Path.Combine(root, "..", "..", "..");
            var gradlePath = Path.Combine(Path.GetFullPath(root), "gradle-6.5");
            Debug.Log(gradlePath);

            if (!Directory.Exists(gradlePath)) {
                throw new Exception("no gradle " + gradlePath);
            }

            var path = EditorPrefs.GetString("GradlePath");
            path = Path.GetFullPath(path);
            Debug.Log(path);

            EditorPrefs.SetBool("GradleUseEmbedded", false);
            EditorPrefs.SetString("GradlePath", gradlePath);
            if (path != gradlePath) {

            }

        }

    }
}