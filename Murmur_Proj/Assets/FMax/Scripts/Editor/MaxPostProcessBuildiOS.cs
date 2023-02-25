#if UNITY_IOS || UNITY_IPHONE

#if UNITY_2019_3_OR_NEWER
using UnityEditor.iOS.Xcode.Extensions;
#endif
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using System.Xml.Linq;

namespace Max.Scripts.Editor
{
    public class MaxPostProcessBuildiOS
    {
        //研发可以自己设置false从而跳过脚本修改xcode工程
        private const bool Enable = true;
        private static readonly string LogFile = string.Join(
            Path.DirectorySeparatorChar.ToString(),
            "Logs/MaxPostProcessBuildiOS.log"
        );

#if !UNITY_2019_3_OR_NEWER
        private const string UnityMainTargetName = "Unity-iPhone";
#endif
        private const string TargetUnityIphonePodfileLine = "target 'Unity-iPhone' do";
        private const string LegacyResourcesDirectoryName = "Resources";
        private const string MaxResourcesDirectoryName = "MaxResources";
        private const string AdvertisingAttributionEndpoint = "https://appsflyer-skadnetwork.com/";

        private const string GoogleServiceInfoPlist = "GoogleService-Info.plist";
        private const string MaxBundleName = "flow998_resource.bundle";

        private const string MaxImport = "#import <FlowHwSDK/FlowHwSDK.h>"; 

        private static readonly Regex TokenDidFinishLaunchingWithOptions = new Regex("(-\\s*\\(BOOL\\)application:\\(UIApplication\\*\\)application\\s+didFinishLaunchingWithOptions:\\(NSDictionary\\*\\)launchOptions[\\s\n]*{)", RegexOptions.Singleline);
        private const string MaxDidFinishLaunchingWithOptions = "[[FHSDKFlow998_SDK shared] applicationDidFinishLaunchingWithOptionsLaunchOptions:launchOptions];";

        private static readonly Regex TokenOpenURL = new Regex("(-\\s*\\(BOOL\\)application:\\(UIApplication\\*\\)app\\s+openURL:\\(NSURL\\*\\)url\\s+options:\\(NSDictionary<NSString\\*,\\s*id>\\*\\)options[\\s\n]*{)", RegexOptions.Singleline);
        private const string MaxOpenURL = "[[FHSDKFlow998_SDK shared] applicationOpenURLUrl:url options:options];";

        private static List<string> DynamicLibraryPathsToEmbed
        {
            get
            {
                var dynamicLibraryPathsToEmbed = new List<string>();
//                dynamicLibraryPathsToEmbed.Add(Path.Combine("Pods/", "FBAEMKit/XCFrameworks/FBAEMKit.xcframework"));
//                dynamicLibraryPathsToEmbed.Add(Path.Combine("Pods/", "FBSDKCoreKit_Basics/XCFrameworks/FBSDKCoreKit_Basics.xcframework"));
//                dynamicLibraryPathsToEmbed.Add(Path.Combine("Pods/", "FBSDKCoreKit/XCFrameworks/FBSDKCoreKit.xcframework"));
//                dynamicLibraryPathsToEmbed.Add(Path.Combine("Pods/", "FBSDKLoginKit/XCFrameworks/FBSDKLoginKit.xcframework"));
//                dynamicLibraryPathsToEmbed.Add(Path.Combine("Pods/", "FBSDKShareKit/XCFrameworks/FBSDKShareKit.xcframework"));
                return dynamicLibraryPathsToEmbed;
            }
        }

        private static List<string> systemFrameworks = new List<string> {
            "Accelerate.framework",
            "AdServices.framework",
            "AdSupport.framework",
            "AppTrackingTransparency.framework",
            "AudioToolbox.framework",
            "AVFoundation.framework",
            "CoreGraphics.framework",
            "CoreMedia.framework",
            "CoreMotion.framework",
            "CoreTelephony.framework",
            "Foundation.framework",
            "MessageUI.framework",
            "SafariServices.framework",
            "StoreKit.framework",
            "SystemConfiguration.framework",
            "UIKit.framework",
            "WebKit.framework",
            "DeviceCheck.framework",
            "libsqlite3.tbd",
            "libz.tbd",
            "libbz2.tbd",
            "libc++abi.tbd",
            "libc++.tbd",
            "libresolv.tbd",
            "libxml2.tbd",
            "libiconv.tbd"
        };

        private static void writeLog(string format, params object[] args)
        {
            File.AppendAllText(LogFile, string.Format(format, args) + "\n");
        }


        [PostProcessBuildAttribute(int.MaxValue)]
        public static void MaxPostProcess(BuildTarget buildTarget, string buildPath)
        {
            File.WriteAllText(LogFile, "Start\n");
            if (!Enable)
            {
                writeLog("Disabled");
                return;
            }
            writeLog("Enabled");
            writeLog("Start ProcessPbxProject");
            ProcessPbxProject(buildTarget, buildPath);
            writeLog("Start ProcessPlist");
            ProcessPlist(buildTarget, buildPath);
        }

        private static void ProcessPbxProject(BuildTarget buildTarget, string buildPath)
        {
            var projectPath = PBXProject.GetPBXProjectPath(buildPath);
            var project = new PBXProject();
            project.ReadFromFile(projectPath);

#if UNITY_2019_3_OR_NEWER
            var unityMainTargetGuid = project.GetUnityMainTargetGuid();
            var unityFrameworkTargetGuid = project.GetUnityFrameworkTargetGuid();
#else
            var unityMainTargetGuid = project.TargetGuidByName(UnityMainTargetName);
            var unityFrameworkTargetGuid = project.TargetGuidByName(UnityMainTargetName);
#endif
            //添加idfa权限的多语言
            writeLog("写入idfa权限文案多语言");
            LocalizeUserTrackingDescriptionIfNeeded("Your data will be used to deliver personalized ads to you.", "en", buildPath, project, unityMainTargetGuid);
            LocalizeUserTrackingDescriptionIfNeeded("Thông tin thiết bị chỉ được sử dụng để đẩy cho bạn những quảng cáo có liên quan và quan tâm hơn", "vi", buildPath, project, unityMainTargetGuid);
            LocalizeUserTrackingDescriptionIfNeeded("ข้อมูลอุปกรณ์ใช้เพื่อผลักดันให้คุณมีโฆษณาที่สนใจและมีความเกี่ยวข้องมากขึ้นเท่านั้น", "th", buildPath, project, unityMainTargetGuid);
            LocalizeUserTrackingDescriptionIfNeeded("あなたのデータはあなたにパーソナライズされた広告を提供するために使用されます", "ja", buildPath, project, unityMainTargetGuid);
            LocalizeUserTrackingDescriptionIfNeeded("设备信息仅用作为您推送更感兴趣且相关的广告", "zh-Hans", buildPath, project, unityMainTargetGuid);
            LocalizeUserTrackingDescriptionIfNeeded("設備信息僅用作為您推送更感興趣且相關的廣告", "zh-Hant", buildPath, project, unityMainTargetGuid);
            //添加依赖的系统库
            writeLog("设置依赖的系统库");
            foreach(var systemFramework in systemFrameworks)
            {
                project.AddFrameworkToProject(unityMainTargetGuid, systemFramework, false);
            }
            //设置动态库
            writeLog("设置依赖的动态库");
            EmbedDynamicLibrariesIfNeeded(buildPath, project, unityMainTargetGuid);
            //添加swift支持
            writeLog("添加swift支持");
            AddSwiftSupportIfNeeded(buildPath, project, unityFrameworkTargetGuid);
            project.AddBuildProperty(unityFrameworkTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
            //关闭bitcode
            writeLog("关闭bitcode");
            project.SetBuildProperty(unityMainTargetGuid, "ENABLE_BITCODE", "NO");
            project.SetBuildProperty(unityFrameworkTargetGuid, "ENABLE_BITCODE", "NO");
            //对Other Linker Flags添加"-ObjC"
            writeLog("Other Linker Flags添加'-ObjC'");
            project.AddBuildProperty(unityFrameworkTargetGuid, "OTHER_CFLAGS", "-ObjC");
            //设置架构 = arm64
            project.SetBuildProperty(unityMainTargetGuid, "ARCHS", "arm64");

            //添加GoogleService-Info.plist
            var googleServiceInfoPlist = string.Join(
                Path.DirectorySeparatorChar.ToString(),
                ("Assets/FMax/Platforms/Plugins/iOS/" + GoogleServiceInfoPlist).Split('/')
            );
            if (File.Exists(googleServiceInfoPlist))
            {
                writeLog("复制{0}", GoogleServiceInfoPlist);
                File.Copy(googleServiceInfoPlist, Path.Combine(buildPath, GoogleServiceInfoPlist), true);
                var googleServiceInfoPlistGuid = project.AddFile(GoogleServiceInfoPlist, GoogleServiceInfoPlist, PBXSourceTree.Source);
                project.AddFileToBuild(unityMainTargetGuid, googleServiceInfoPlistGuid);
            }
            //添加bundle
            var bundle = string.Join(
                Path.DirectorySeparatorChar.ToString(),
                ("Assets/FMax/Platforms/Plugins/iOS/" + MaxBundleName).Split('/')
            );
            if (Directory.Exists(bundle))
            {
                writeLog("复制{0}", MaxBundleName);
                var targetBundle = Path.Combine(buildPath, MaxBundleName);
                if (!Directory.Exists(targetBundle))
                {
                    Directory.CreateDirectory(targetBundle);
                }
                CopyFilesRecursively(bundle, targetBundle);
                var bundleGuid = project.AddFolderReference(MaxBundleName, MaxBundleName);
                project.AddFileToBuild(unityMainTargetGuid, bundleGuid);
            }

            //接入生命周期
            writeLog("开始接入生命周期");
            var UnityAppControllermm = Path.Combine(buildPath, "Classes/UnityAppController.mm");
            var UnityAppController = File.ReadAllText(UnityAppControllermm);
            var flag = false;
            if (!UnityAppController.Contains(MaxImport))
            {
                writeLog("接入 = {0}", MaxImport);
                UnityAppController = MaxImport + "\n\n" + UnityAppController;
                flag = true;
            }
            if (!UnityAppController.Contains(MaxDidFinishLaunchingWithOptions))
            {
                writeLog("接入 - {0}", MaxDidFinishLaunchingWithOptions);
                UnityAppController = TokenDidFinishLaunchingWithOptions.Replace(UnityAppController, "$1\n\t" + MaxDidFinishLaunchingWithOptions);
                flag = true;
            }
            if (!UnityAppController.Contains(MaxOpenURL))
            {
                writeLog("接入 - {0}", MaxOpenURL);
                UnityAppController = TokenOpenURL.Replace(UnityAppController, "$1\n\t" + MaxOpenURL);
                flag = true;
            }
            if (flag)
            {
                File.WriteAllText(UnityAppControllermm, UnityAppController);
            }
            
            project.WriteToFile(projectPath);
        }

        private static void LocalizeUserTrackingDescriptionIfNeeded(string localizedUserTrackingDescription, string localeCode, string buildPath, PBXProject project, string targetGuid)
        {
            // Use the legacy resources directory name if the build is being appended (the "Resources" directory already exists if it is an incremental build).
            var resourcesDirectoryName = Directory.Exists(Path.Combine(buildPath, LegacyResourcesDirectoryName)) ? LegacyResourcesDirectoryName : MaxResourcesDirectoryName;
            var resourcesDirectoryPath = Path.Combine(buildPath, resourcesDirectoryName);
            var localeSpecificDirectoryName = localeCode + ".lproj";
            var localeSpecificDirectoryPath = Path.Combine(resourcesDirectoryPath, localeSpecificDirectoryName);
            var infoPlistStringsFilePath = Path.Combine(localeSpecificDirectoryPath, "InfoPlist.strings");
            // Create intermediate directories as needed.
            if (!Directory.Exists(resourcesDirectoryPath))
            {
                Directory.CreateDirectory(resourcesDirectoryPath);
            }
            if (!Directory.Exists(localeSpecificDirectoryPath))
            {
                Directory.CreateDirectory(localeSpecificDirectoryPath);
            }
            var localizedDescriptionLine = "\"NSUserTrackingUsageDescription\" = \"" + localizedUserTrackingDescription + "\";\n";
            // File already exists, update it in case the value changed between builds.
            if (File.Exists(infoPlistStringsFilePath))
            {
                var output = new List<string>();
                var lines = File.ReadAllLines(infoPlistStringsFilePath);
                var keyUpdated = false;
                foreach (var line in lines)
                {
                    if (line.Contains("NSUserTrackingUsageDescription"))
                    {
                        output.Add(localizedDescriptionLine);
                        keyUpdated = true;
                    }
                    else
                    {
                        output.Add(line);
                    }
                }

                if (!keyUpdated)
                {
                    output.Add(localizedDescriptionLine);
                }

                File.WriteAllText(infoPlistStringsFilePath, string.Join("\n", output.ToArray()) + "\n");
            }
            // File doesn't exist, create one.
            else
            {
                File.WriteAllText(infoPlistStringsFilePath, "/* Localized versions of Info.plist keys - Generated by AL MAX plugin */\n" + localizedDescriptionLine);
            }
            var localeSpecificDirectoryRelativePath = Path.Combine(resourcesDirectoryName, localeSpecificDirectoryName);
            var guid = project.AddFolderReference(localeSpecificDirectoryRelativePath, localeSpecificDirectoryRelativePath);
            project.AddFileToBuild(targetGuid, guid);
        }

        private static void EmbedDynamicLibrariesIfNeeded(string buildPath, PBXProject project, string targetGuid)
        {
            var dynamicLibraryPathsPresentInProject = DynamicLibraryPathsToEmbed.Where(dynamicLibraryPath => Directory.Exists(Path.Combine(buildPath, dynamicLibraryPath))).ToList();
            if (dynamicLibraryPathsPresentInProject.Count <= 0)
            {
                return;
            }
#if UNITY_2019_3_OR_NEWER
            // Embed framework only if the podfile does not contain target `Unity-iPhone`.
            if (!ContainsUnityIphoneTargetInPodfile(buildPath))
            {
                foreach (var dynamicLibraryPath in dynamicLibraryPathsPresentInProject)
                {
                    var fileGuid = project.AddFile(dynamicLibraryPath, dynamicLibraryPath);
                    project.AddFileToEmbedFrameworks(targetGuid, fileGuid);
                }
            }
#else
            string runpathSearchPaths;
#if UNITY_2018_2_OR_NEWER
            runpathSearchPaths = project.GetBuildPropertyForAnyConfig(targetGuid, "LD_RUNPATH_SEARCH_PATHS");
#else
            runpathSearchPaths = "$(inherited)";          
#endif
            runpathSearchPaths += string.IsNullOrEmpty(runpathSearchPaths) ? "" : " ";

            // Check if runtime search paths already contains the required search paths for dynamic libraries.
            if (runpathSearchPaths.Contains("@executable_path/Frameworks")) return;

            runpathSearchPaths += "@executable_path/Frameworks";
            project.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", runpathSearchPaths);
#endif
        }

        private static void AddSwiftSupportIfNeeded(string buildPath, PBXProject project, string targetGuid)
        {
            var dummySwiftFilePath = Path.Combine(buildPath, "Dummy.swift");
            if (File.Exists(dummySwiftFilePath))
            {
                return;
            }
            var swiftFileRelativePath = "Classes/MaxSwiftSupport.swift";
            var swiftFilePath = Path.Combine(buildPath, swiftFileRelativePath);
            // Add Swift file
            CreateSwiftFile(swiftFilePath);
            var swiftFileGuid = project.AddFile(swiftFileRelativePath, swiftFileRelativePath, PBXSourceTree.Source);
            project.AddFileToBuild(targetGuid, swiftFileGuid);

            // Add Swift version property if needed
#if UNITY_2018_2_OR_NEWER
            var swiftVersion = project.GetBuildPropertyForAnyConfig(targetGuid, "SWIFT_VERSION");
#else
            // Assume that swift version is not set on older versions of Unity.
            const string swiftVersion = "";
#endif
            if (string.IsNullOrEmpty(swiftVersion))
            {
                project.SetBuildProperty(targetGuid, "SWIFT_VERSION", "5.0");
            }
            // Enable Swift modules
            project.AddBuildProperty(targetGuid, "CLANG_ENABLE_MODULES", "YES");
        }

        private static void CreateSwiftFile(string swiftFilePath)
        {
            if (File.Exists(swiftFilePath)) return;

            // Create a file to write to.
            using (var writer = File.CreateText(swiftFilePath))
            {
                writer.WriteLine("//\n//  MaxSwiftSupport.swift\n//");
                writer.WriteLine("\nimport Foundation\n");
                writer.WriteLine("// This file ensures the project includes Swift support.");
                writer.WriteLine("// It is automatically generated by the FMax Unity Plugin.");
                writer.Close();
            }
        }

        private static void ProcessPlist(BuildTarget buildTarget, string path)
        {
            var plistPath = Path.Combine(path, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            var sdkInfoPlistPath = string.Join(
                Path.DirectorySeparatorChar.ToString(),
                ("Assets/FMax/Platforms/Plugins/iOS/InfoPlist.xml").Split('/')
            );
            var sdkInfoPlist = new PlistDocument();
            sdkInfoPlist.ReadFromFile(sdkInfoPlistPath);

            foreach (var item in sdkInfoPlist.root.values)
            {
                if (item.Value.GetType() == typeof(PlistElementString))
                {
                    var str = item.Value.AsString();
                    writeLog("plist[{0}] = {1}", item.Key, str);
                    plist.root.SetString(item.Key, str);
                }
                else if (item.Key == "NSAppTransportSecurity")
                {
                    continue;
                }
                else if (item.Key == "SKAdNetworkItems")
                {
                    var plistElementDictionaries = item.Value.AsArray().values.Where(plistElement => plistElement.GetType() == typeof(PlistElementDict));
                    var skAdNetworkIds = new List<string>();
                    foreach (var plistElement in plistElementDictionaries)
                    {
                        PlistElement existingId;
                        plistElement.AsDict().values.TryGetValue("SKAdNetworkIdentifier", out existingId);
                        if (existingId == null ||
                            existingId.GetType() != typeof(PlistElementString) ||
                            string.IsNullOrEmpty(existingId.AsString()))
                        {
                            continue;
                        }
                        skAdNetworkIds.Add(existingId.AsString());
                    }
                    writeLog("plist[{0}]长度 = {1}", item.Key, skAdNetworkIds.Count);
                    AddSKAdNetworkItems(plist, skAdNetworkIds);
                }
                else if (item.Key == "CFBundleURLTypes")
                {
                    var arr = item.Value.AsArray();
                    if (arr.values.Count == 0)
                    {
                        continue;
                    }
                    var schemes = new List<string>();
                    var types = item.Value.AsArray().values.Where(plistElement => plistElement.GetType() == typeof(PlistElementDict));
                    foreach (var type in types)
                    {
                        PlistElement oldSchemes;
                        type.AsDict().values.TryGetValue("CFBundleURLSchemes", out oldSchemes);
                        if (oldSchemes != null && oldSchemes.GetType() == typeof(PlistElementArray))
                        {
                            foreach (var oldScheme in oldSchemes.AsArray().values)
                            {
                                if (oldScheme.GetType() == typeof(PlistElementString))
                                {
                                    schemes.Add(oldScheme.AsString());
                                }
                            }
                        }
                    }
                    writeLog("plist[{0}] = {1}", item.Key, string.Join(",", schemes));
                    AddCFBundleURLTypes(plist, schemes);
                }
                else if (item.Key == "LSApplicationQueriesSchemes")
                {
                    var schemes = new List<string>();
                    var schemeElements = item.Value.AsArray().values.Where(plistElement => plistElement.GetType() == typeof(PlistElementString));
                    foreach (var scheme in schemeElements)
                    {
                        schemes.Add(scheme.AsString());
                    }
                    writeLog("plist[{0}] = {1}", item.Key, string.Join(",", schemes));
                    AddLSApplicationQueriesSchemes(plist, schemes);
                }
            }
            //设置运行http访问
            UpdateNSAppTransportSecurity(plist);

            plist.WriteToFile(plistPath);
        }

        private static void AddSKAdNetworkItems(PlistDocument plist, List<string> skAdNetworkIds)
        {
            if (skAdNetworkIds.Count == 0)
            {
                return;
            }
            PlistElement skAdNetworkItems;
            plist.root.values.TryGetValue("SKAdNetworkItems", out skAdNetworkItems);
            var existingSkAdNetworkIds = new HashSet<string>();
            // Check if SKAdNetworkItems array is already in the Plist document and collect all the IDs that are already present.
            if (skAdNetworkItems != null && skAdNetworkItems.GetType() == typeof(PlistElementArray))
            {
                var plistElementDictionaries = skAdNetworkItems.AsArray().values.Where(plistElement => plistElement.GetType() == typeof(PlistElementDict));
                foreach (var plistElement in plistElementDictionaries)
                {
                    PlistElement existingId;
                    plistElement.AsDict().values.TryGetValue("SKAdNetworkIdentifier", out existingId);
                    if (existingId == null || existingId.GetType() != typeof(PlistElementString) || string.IsNullOrEmpty(existingId.AsString())) continue;

                    existingSkAdNetworkIds.Add(existingId.AsString());
                }
            }
            // Else, create an array of SKAdNetworkItems into which we will add our IDs.
            else
            {
                skAdNetworkItems = plist.root.CreateArray("SKAdNetworkItems");
            }
            foreach (var skAdNetworkId in skAdNetworkIds)
            {
                // Skip adding IDs that are already in the array.
                if (existingSkAdNetworkIds.Contains(skAdNetworkId)) continue;

                var skAdNetworkItemDict = skAdNetworkItems.AsArray().AddDict();
                skAdNetworkItemDict.SetString("SKAdNetworkIdentifier", skAdNetworkId);
            }
        }

        private static void AddLSApplicationQueriesSchemes(PlistDocument plist, List<string> schemes)
        {
            if (schemes.Count == 0)
            {
                return;
            }
            PlistElement element;
            plist.root.values.TryGetValue("LSApplicationQueriesSchemes", out element);
            var oldSchemeSet = new HashSet<string>();
            if (element == null || element.GetType() != typeof(PlistElementArray))
            {
                element = plist.root.CreateArray("LSApplicationQueriesSchemes");
                
            } else
            {
                var oldSchemes = element.AsArray().values.Where(plistElement => plistElement.GetType() == typeof(PlistElementString));
                foreach(var scheme in oldSchemes)
                {
                    oldSchemeSet.Add(scheme.AsString());
                }
            }
            foreach (var scheme in schemes)
            {
                if (oldSchemeSet.Contains(scheme))
                {
                    continue;
                }
                element.AsArray().AddString(scheme);
            }
        }

        private static void AddCFBundleURLTypes(PlistDocument plist, List<string> schemes)
        {
            if (schemes.Count == 0)
            {
                return;
            }
            PlistElement element;
            plist.root.values.TryGetValue("CFBundleURLTypes", out element);
            var oldSchemeSet = new HashSet<string>();
            if (element == null || element.GetType() != typeof(PlistElementArray))
            {
                element = plist.root.CreateArray("CFBundleURLTypes");
            } else
            {
                var types = element.AsArray().values.Where(plistElement => plistElement.GetType() == typeof(PlistElementDict));
                foreach (var type in types)
                {
                    PlistElement oldSchemes;
                    type.AsDict().values.TryGetValue("CFBundleURLSchemes", out oldSchemes);
                    if (oldSchemes != null && oldSchemes.GetType() == typeof(PlistElementArray))
                    {
                        foreach(var oldScheme in oldSchemes.AsArray().values)
                        {
                            if (oldScheme.GetType() == typeof(PlistElementString))
                            {
                                oldSchemeSet.Add(oldScheme.AsString());
                            }
                        }
                    }
                }
            }
            var schemes1 = schemes.Where(scheme => !oldSchemeSet.Contains(scheme));
            if (schemes1.Count() == 0)
            {
                return;
            }
            var dict = element.AsArray().AddDict();
            dict.SetString("CFBundleTypeRole", "Editor");
            var arr = dict.CreateArray("CFBundleURLSchemes");
            foreach(var scheme in schemes1)
            {
                arr.AddString(scheme);
            }
        }

        private static void UpdateNSAppTransportSecurity(PlistDocument plist)
        {
            var root = plist.root.values;
            PlistElement atsRoot;
            root.TryGetValue("NSAppTransportSecurity", out atsRoot);
            if (atsRoot == null || atsRoot.GetType() != typeof(PlistElementDict))
            {
                // Add the missing App Transport Security settings for publishers if needed. 
                atsRoot = plist.root.CreateDict("NSAppTransportSecurity");
                atsRoot.AsDict().SetBoolean("NSAllowsArbitraryLoads", true);
            }
            var atsRootDict = atsRoot.AsDict().values;
            // Check if both NSAllowsArbitraryLoads and NSAllowsArbitraryLoadsInWebContent are present and remove NSAllowsArbitraryLoadsInWebContent if both are present.
            if (atsRootDict.ContainsKey("NSAllowsArbitraryLoads") && atsRootDict.ContainsKey("NSAllowsArbitraryLoadsInWebContent"))
            {
                atsRootDict.Remove("NSAllowsArbitraryLoadsInWebContent");
            }
        }

#if UNITY_2019_3_OR_NEWER
        private static bool ContainsUnityIphoneTargetInPodfile(string buildPath)
        {
            var podfilePath = Path.Combine(buildPath, "Podfile");
            if (!File.Exists(podfilePath)) return false;

            var tokenPod = new Regex("^pod\\s+'\"");

            var lines = File.ReadAllLines(podfilePath);
            var flag = 0;
            foreach(var line in lines)
            {
                if (flag == 0)
                {
                    if (line.Contains(TargetUnityIphonePodfileLine))
                    {
                        flag = 1;
                    }
                } else if (flag == 1)
                {
                    var line1 = line.Trim();
                    if (line1 == "end")
                    {
                        break;
                    } else
                    {
                        if (tokenPod.IsMatch(line1))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
#endif

        private static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }
            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }
    }
}

#endif
