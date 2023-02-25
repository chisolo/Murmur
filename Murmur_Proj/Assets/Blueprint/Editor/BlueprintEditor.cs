using System.Collections.Generic;
using System.IO;
using System.Linq;
using LitJson;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.AddressableAssets.Settings;
using Lemegeton;

public class BlueprintEditor : Editor
{
    [MenuItem("Lemegeton/Blueprint/Mod/Generate")]
    public static void GenerateModConfig()
    {
        var scene = SceneManager.GetActiveScene();
        var filePath = GameUtil.ResModConfigPath;

        var rootObjects = scene.GetRootGameObjects();
        var mods = new List<ModHelper>();
        foreach(var rootObject in rootObjects) {
            var children = rootObject.GetComponentsInChildren<ModHelper>();
            mods.AddRange(children);
        }
        Dictionary<string, ModConfig> modConfigs = new Dictionary<string, ModConfig>();
        foreach(var mod in mods) {
            var modConfig = new ModConfig() {
                id = mod.gameObject.name,
                position = mod.transform.position,
                rotation = mod.transform.rotation,
                scale = mod.transform.lossyScale,
                asset = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(mod.gameObject))
            };
            modConfigs.Add(modConfig.id, modConfig);
        }
        if(modConfigs.Count == 0) {
            EditorUtility.DisplayDialog("Generate Mods", "No mod found! Please check scene", "Get it");
            return;
        }

        var dataStr = EncryptUtil.DESEncrypt(JsonMapper.ToJson(modConfigs), GameUtil.ResModCryptKey);
        if (File.Exists(filePath)) {
            AssetDatabase.DeleteAsset(filePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        File.WriteAllText(filePath, dataStr);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        //AddAssetToAddressables(filePath);
        EditorUtility.DisplayDialog("Generate Mods", "All Mods Done", "OK");
    }
    [MenuItem("Lemegeton/Blueprint/Spot/Show")]
    public static void ShowSpots()
    {
        var scene = SceneManager.GetActiveScene();
        var rootObjects = scene.GetRootGameObjects();

        foreach(var rootObject in rootObjects) {
            var children = rootObject.GetComponentsInChildren<SpotHelper>();
            foreach(var child in children) {
                child.gameObject.SetActive(true);
            }
        }
    }
    [MenuItem("Lemegeton/Blueprint/Spot/Hide")]
    public static void HideSpots()
    {
        var scene = SceneManager.GetActiveScene();
        var rootObjects = scene.GetRootGameObjects();

        foreach(var rootObject in rootObjects) {
            var children = rootObject.GetComponentsInChildren<SpotHelper>();
            foreach(var child in children) {
                child.gameObject.SetActive(false);
            }
        }
    }
    [MenuItem("Lemegeton/Blueprint/Spot/Generate")]
    public static void GenerateSpotConifg()
    {
        var scene = SceneManager.GetActiveScene();
        var fileName = scene.name;
        var filePath = GameUtil.ResSpotConfigPath;

        var rootObjects = scene.GetRootGameObjects();
        var spots = new List<SpotHelper>();
        foreach(var rootObject in rootObjects) {
            var children = rootObject.GetComponentsInChildren<SpotHelper>();
            spots.AddRange(children);
        }

        Dictionary<string, SpotConfig> spotConfigs = new Dictionary<string, SpotConfig>();
        foreach(var spot in spots) {
            var spotConfig = new SpotConfig() {
                id = spot.gameObject.name,
                position = spot.transform.position,
                rotation = spot.transform.rotation,
                strict = spot.strict
            };
            spotConfigs.Add(spotConfig.id, spotConfig);
        }
        if(spotConfigs.Count == 0) {
            EditorUtility.DisplayDialog("Generate Spots", "No spot found! Please check scene", "Get it");
            return;
        }

        var dataStr = EncryptUtil.DESEncrypt(JsonMapper.ToJson(spotConfigs), GameUtil.ResSpotCryptKey);
        if (File.Exists(filePath)) {
            AssetDatabase.DeleteAsset(filePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        File.WriteAllText(filePath, dataStr);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        //AddAssetToAddressables(filePath);
        EditorUtility.DisplayDialog("Generate Spots", "All Spots Done", "OK");
    }

    private static AddressableAssetSettings GetAddressableSetting()
    {
        var guid = AssetDatabase.FindAssets("t:AddressableAssetSettings").FirstOrDefault();
        var path = AssetDatabase.GUIDToAssetPath(guid);
        return AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(path);
    }
    private static void AddAssetToAddressables(string path)
    {
        var setting = GetAddressableSetting();
        var group = setting.groups.Find(e => e.name == "Config");
        if(group == null) Debug.LogError("Group Config Not Find!");
        var entry = setting.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), group, false, true);
        entry.SetAddress(path, true);
    }
}
