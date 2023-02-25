using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using UnityEditor.AddressableAssets.Settings;

namespace App
{
    public class AppPostBuild : IPostprocessBuildWithReport
    {

        public int callbackOrder { get { return 0; } }
        public void OnPostprocessBuild(BuildReport report)
        {
            Debug.Log("AppPostBuild.OnPostprocessBuild for target " + report.summary.platform + " at path " + report.summary.outputPath);

        }
    }
}