using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace App.Dev
{
    public class DevAssetsLoader
    {
#if IS_DEV
        public static DevAssets devAssets;

        static DevAssetsLoader()
        {
            AppLogger.Log("DevAssetsLoader int");

            devAssets = Resources.Load<DevAssets>("DevAssets");
        }
#endif
    }
}