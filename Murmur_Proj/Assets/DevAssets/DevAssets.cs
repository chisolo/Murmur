using UnityEngine;

namespace App.Dev
{
    [CreateAssetMenu(fileName = "DevAssets", menuName = "ScriptableObjects/DevAssets")]
    public class DevAssets : ScriptableObject
    {
        public GameObject IngameDebugConsolePrefab;
    }
}
