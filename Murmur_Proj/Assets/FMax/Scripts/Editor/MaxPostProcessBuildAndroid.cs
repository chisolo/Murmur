#if UNITY_ANDROID && UNITY_2018_2_OR_NEWER

using System.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Android;
using NUnit.Framework.Interfaces;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FMax.Scripts.Editor
{
    public class MaxPostProcessBuildAndroid : IPostGenerateGradleAndroidProject
    {
        //研发可以自己设置false从而跳过脚本修改android工程
        private const bool Enable = true;
        private const string Platform = "firebase";
        private static readonly string LogFile = string.Join(
            Path.DirectorySeparatorChar.ToString(),
            "Logs/MaxPostProcessBuildAndroid.log"
        );

        private static readonly Regex TokenBuildGradleRepositories = new Regex("( +)(google\\(\\))");
        private const string BuildGradleRepositories = @"$1$2
$1maven {
$1    url 'http://47.75.51.140:10101/e6091109cbc8d4de9058/repository/flowhw-group/'
$1    allowInsecureProtocol = true
$1    credentials {
$1        username = 'flowhw'
$1        password = 'A5sAtOy-sccdI-F'
$1    }
$1}";

        private static readonly Regex TokenBuildGradleClasspath = new Regex("( +)(classpath\\s+\'com\\.android\\.tools\\.build:gradle:[^\']+\')");
        private const string BuildGradleClasspathsFirebase = @"$1$2
$1classpath 'com.flowhw.sdk:gradle-plugin:+'
$1classpath 'com.applovin.quality:AppLovinQualityServiceGradlePlugin:+'
$1classpath 'com.google.firebase:firebase-crashlytics-gradle:2.9.1'
$1classpath 'com.google.gms:google-services:4.3.13'";
        private const string BuildGradleClasspathsHuawei = @"$1$2
$1classpath 'com.huawei.agconnect:agcp:1.7.1.300'";

        private static readonly Regex TokenBuildGradleApplyPlugin = new Regex("(apply\\s+plugin\\s*:\\s+\'com\\.android\\.application\')");
        private const string BuildGradleApplyPluginsFirebase = @"$1
apply plugin: 'com.flowhw.sdk.gradle-plugin'
apply plugin: 'com.google.firebase.crashlytics'
apply plugin: 'applovin-quality-service'
apply plugin: 'com.google.gms.google-services'";
        private const string BuildGradleApplyPluginsHuawei = @"$1
apply plugin: 'com.huawei.agconnect'";

        private static readonly Regex TokenGradleVersion = new Regex("distributions\\/gradle\\-([0-9.]+)");
        private static readonly Regex TokenGradleToolsVersion = new Regex("com\\.android\\.tools\\.build:gradle:([0-9.]+)");

        private static readonly Regex TokenBuildGradleApplovin = new Regex(".*applovin\\s*\\{.*");
        private const string BuildGradleApplovin = @"applovin {
    apiKey 'PayrECDoAxOvTCQI9TeeR45ryFTm6CWCHawEaEgetOxNXD3pDACtdL1aG9V_1ITLyZ1T8M3vzMC_n2a_uCxNWI'
}";

        private static readonly Regex TokenBuildGradleDependencies1 = new Regex(".*dependencies\\s*\\{.*");
        private static readonly Regex TokenBuildGradleDependencies2 = new Regex("(dependencies\\s*\\{)");

#if UNITY_2019_3_OR_NEWER
        private const string GradlePropertyAndroidX = "android.useAndroidX";
        private const string GradlePropertyJetifier = "android.enableJetifier";
        private const string GradlePropertyEnable = "=true";
#endif
        private const string GradlePropertyDexingArtifactTransform = "android.enableDexingArtifactTransform";
        private const string GradlePropertyDisable = "=false";

        private static void writeLog(string format, params object[] args)
        {
            File.AppendAllText(LogFile, string.Format(format, args) + "\n");
        }

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            File.WriteAllText(LogFile, "Start\n");
            if (!Enable)
            {
                writeLog("MaxPostProcessBuildAndroid disable - {0}", Platform);
                return;
            }
            writeLog("MaxPostProcessBuildAndroid enable - {0}", Platform);
#if UNITY_2019_3_OR_NEWER
            var rootGradleBuildFilePath = Path.Combine(path, "../build.gradle");
            var applicationGradleBuildFilePath = Path.Combine(path, "../launcher/build.gradle");
            var libraryGradleBuildFilePath = Path.Combine(path, "../unityLibrary/build.gradle");
            var gradlePropertiesPath = Path.Combine(path, "../gradle.properties");
#else
            var rootGradleBuildFilePath = Path.Combine(path, "build.gradle");
            var applicationGradleBuildFilePath = rootGradleBuildFilePath;
            var libraryGradleBuildFilePath = rootGradleBuildFilePath;
            var gradlePropertiesPath = Path.Combine(path, "gradle.properties");
#endif
            writeLog("修改root build.gradle");
            BuildGradleRootHandle(rootGradleBuildFilePath);
            writeLog("修改application build.gradle");
            BuildGradleApplicationHandle(applicationGradleBuildFilePath);
            writeLog("修改library build.gradle");
            BuildGradleLibraryHandle(libraryGradleBuildFilePath);
            writeLog("修改gradle.properties");
            GradlePropertiesHandle(gradlePropertiesPath);
            writeLog("复制文件");
            FilesCopyApplication(applicationGradleBuildFilePath);
            writeLog("修改AndroidManifest.xml");
            AndroidManifestHandle(path);
        }

        public int callbackOrder
        {
            get { return int.MaxValue; }
        }

        private void BuildGradleRootHandle(string path)
        {
            string content = File.ReadAllText(path);
            content = TokenBuildGradleRepositories.Replace(content, BuildGradleRepositories);
            if (Platform == "huawei")
            {
                content = TokenBuildGradleClasspath.Replace(content, BuildGradleClasspathsHuawei);
            }
            else
            {
                content = TokenBuildGradleClasspath.Replace(content, BuildGradleClasspathsFirebase);
            }
            File.WriteAllText(path, content);
            //修改gradle的版本
            writeLog("修改gradle的版本");
            var gradleWrapperPath = Path.Combine(
                Path.GetDirectoryName(path),
                string.Join(
                    Path.DirectorySeparatorChar.ToString(),
                    "gradle/wrapper/gradle-wrapper.properties".Split('/')
                )
            );
            if (File.Exists(gradleWrapperPath))
            {
                content = File.ReadAllText(gradleWrapperPath);
                var match1 = TokenGradleVersion.Match(content);
                var gradleVersion = new Version(match1.Groups[1].Value);
                if (gradleVersion.CompareTo(new Version("6.5")) < 0)
                {
                    content = TokenGradleVersion.Replace(content, "distributions/gradle-6.5");
                    File.WriteAllText(gradleWrapperPath, content);
                }
            }
            else
            {
                var dir1 = Path.GetDirectoryName(gradleWrapperPath);
                var dir2 = Path.GetDirectoryName(dir1);
                if (!Directory.Exists(dir2))
                {
                    Directory.CreateDirectory(dir2);
                }
                if (!Directory.Exists(dir1))
                {
                    Directory.CreateDirectory(dir1);
                }
                content = @"distributionBase=GRADLE_USER_HOME
distributionPath=wrapper/dists
distributionUrl=https\://services.gradle.org/distributions/gradle-6.5-bin.zip
zipStoreBase=GRADLE_USER_HOME
zipStorePath=wrapper/dists
";
                File.WriteAllText(gradleWrapperPath, content);
            }

            content = File.ReadAllText(path);
            var match2 = TokenGradleToolsVersion.Match(content);
            var gradleToolsVersion = new Version(match2.Groups[1].Value);
            if (gradleToolsVersion.CompareTo(new Version("3.6.4")) < 0)
            {
                content = TokenGradleToolsVersion.Replace(content, "com.android.tools.build:gradle:3.6.4");
                File.WriteAllText(path, content);
            }
        }

        private void BuildGradleApplicationHandle(string path)
        {
            string content = File.ReadAllText(path);
            if (Platform == "huawei")
            {
                content = TokenBuildGradleApplyPlugin.Replace(content, BuildGradleApplyPluginsHuawei);
            }
            else
            {
                content = TokenBuildGradleApplyPlugin.Replace(content, BuildGradleApplyPluginsFirebase);
                if (!TokenBuildGradleApplovin.IsMatch(content))
                {
                    content = content + "\n" + BuildGradleApplovin + "\n";
                }
            }
            File.WriteAllText(path, content);
        }

        private void BuildGradleLibraryHandle(string path)
        {
            string content = File.ReadAllText(path);
            //检查dependencies是否全部配置成功
            var addDependencies = new List<string>();
            var dependenciesNames = new List<string> { "DependenciesCommon.xml" };
            if (Platform == "huawei")
            {
                dependenciesNames.Add("DependenciesHuawei.xml");
            }
            else
            {
                dependenciesNames.Add("DependenciesFirebase.xml");
            }
            foreach (var dependenciesName in dependenciesNames)
            {
                var dependenciesPath = string.Join(
                    Path.DirectorySeparatorChar.ToString(),
                    ("Assets/FMax/Platforms/Plugins/Android/" + dependenciesName).Split('/')
                );
                var dependencies = XDocument.Load(dependenciesPath).Element("dependencies").Element("androidPackages").Elements("androidPackage");
                foreach (var dependency in dependencies)
                {
                    var attr = dependency.Attribute("spec");
                    if (attr == null)
                    {
                        continue;
                    }
                    var dep1 = attr.Value;
                    var dep2 = dep1.Replace(':', '-');
                    var token1 = new Regex("implementation\\(.*" + Regex.Escape(dep1) + ".*");
                    var token2 = new Regex("implementation\\(.*" + Regex.Escape(dep2) + ".*");
                    if (!token1.IsMatch(content) || !token2.IsMatch(content))
                    {
                        if (dep1.StartsWith("platform("))
                        {
                            addDependencies.Add("    implementation(" + dep1 + ")");
                        }
                        else
                        {
                            addDependencies.Add("    implementation('" + dep1 + "')");
                        }
                    }
                }
            }
            if (addDependencies.Count > 0)
            {
                if (TokenBuildGradleDependencies1.IsMatch(content))
                {
                    content = TokenBuildGradleDependencies2.Replace(
                        content,
                        "$1\n" + string.Join("\n", addDependencies) + "\n"
                    );
                }
                else
                {
                    content = content + "\ndependencies {\n"
                        + string.Join("\n", addDependencies.ToArray()) + "\n}\n";
                }
                File.WriteAllText(path, content);
            }
        }

        private void GradlePropertiesHandle(string path)
        {
            var lines = File.ReadAllLines(path);
            var lines1 = new List<string>();

#if UNITY_2019_3_OR_NEWER
            // Add all properties except AndroidX, Jetifier, and DexingArtifactTransform since they may already exist. We will re-add them below.
            lines1.AddRange(lines.Where(line => !line.Contains(GradlePropertyAndroidX) && !line.Contains(GradlePropertyJetifier) && !line.Contains(GradlePropertyDexingArtifactTransform)));
#else
            // Add all properties except DexingArtifactTransform since it may already exist. We will re-add it below.
            lines1.AddRange(lines.Where(line => !line.Contains(GradlePropertyDexingArtifactTransform)));
#endif
#if UNITY_2019_3_OR_NEWER
            // Enable AndroidX and Jetifier properties
            lines1.Add(GradlePropertyAndroidX + GradlePropertyEnable);
            lines1.Add(GradlePropertyJetifier + GradlePropertyEnable);
#endif
            // Disable dexing using artifact transform (it causes issues for ExoPlayer with Gradle plugin 3.5.0+)
            lines1.Add(GradlePropertyDexingArtifactTransform + GradlePropertyDisable);
            File.WriteAllText(path, string.Join("\n", lines1.ToArray()) + "\n");
        }

        private void FilesCopyApplication(string path)
        {
            var dir = Path.GetDirectoryName(path);
            string targetFilename;
            if (Platform == "huawei")
            {
                targetFilename = "agconnect-services.json";
            }
            else
            {
                targetFilename = "google-services.json";
            }
            var targetFile = string.Join(
                Path.DirectorySeparatorChar.ToString(),
                "Assets/FMax/Platforms/Plugins/Android".Split('/')
            ) + Path.DirectorySeparatorChar + targetFilename;
            if (File.Exists(targetFile))
            {
                File.Copy(targetFile, dir + Path.DirectorySeparatorChar + targetFilename, true);
            }
        }

        private void AndroidManifestHandle(string path)
        {
            var manifestPath = Path.Combine(
                path,
                string.Join(
                    Path.DirectorySeparatorChar.ToString(),
                    "src/main/AndroidManifest.xml".Split('/')
                )
            );
            XDocument manifest = XDocument.Load(manifestPath);
            var application = manifest.Element("manifest").Element("application");
            var activities = application.Elements("activity");
            var found = false;
            XNamespace ns = "http://schemas.android.com/apk/res/android";
            foreach (var activity in activities)
            {
                var activityString = activity.ToString();
                if (activityString.Contains("android:name=\"android.intent.action.MAIN\""))
                {
                    activity.SetAttributeValue(ns + "theme", "@android:style/Theme.NoTitleBar.Fullscreen");
                    activity.SetAttributeValue(ns + "name", "com.flowhw.sdk.Flow998_UnityActivity");
                    found = true;
                    break;
                }
            }
            if (found)
            {
                manifest.Save(manifestPath);
            }
        }
    }
}

#endif