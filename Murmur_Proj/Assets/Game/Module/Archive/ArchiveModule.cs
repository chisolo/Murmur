using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lemegeton;
using LitJson;

public class ArchiveModule : Singleton<ArchiveModule>
{
    protected ArchiveModule() { }
    private bool _inited = false;
    private static string _encryptKey = "";
    private Dictionary<string, IArchive> _archives;

    public void Init()
    {
        if(_inited) return;

        _archives = new Dictionary<string, IArchive>();
        _inited = true;
    }

    // 每个Archive至少对应3个函数：
    // 合并优化后续再考虑
    public TArchive GetArchive<TArchive>() where TArchive : class, IArchive, new()
    {
        var fullName = typeof(TArchive).FullName;
        if(_archives.ContainsKey(fullName)) return _archives[fullName] as TArchive;

        TArchive archive;
        if(PlayerPrefs.HasKey(fullName)) {
            var dataStr = PlayerPrefs.GetString(fullName);
            var jsonStr = EncryptUtil.DESDecrypt(dataStr, _encryptKey);
            archive = JsonMapper.ToObject<TArchive>(jsonStr);
        } else {
            archive = new TArchive();
            archive.Default();
        }
        if (archive == null) {
            archive = new TArchive();
            archive.Default();
        }
        _archives.Add(fullName, archive);
        return archive;
    }
    public TArchive GetArchive<TArchive>(string prefix) where TArchive : class, IArchive, new()
    {
        var fullName = prefix + typeof(TArchive).FullName;
        if(_archives.ContainsKey(fullName)) return _archives[fullName] as TArchive;

        TArchive archive;
        if(PlayerPrefs.HasKey(fullName)) {
            var dataStr = PlayerPrefs.GetString(fullName);
            var jsonStr = EncryptUtil.DESDecrypt(dataStr, _encryptKey);
            archive = JsonMapper.ToObject<TArchive>(jsonStr);
        } else {
            archive = new TArchive();
            archive.Default();
        }
        if (archive == null) {
            archive = new TArchive();
            archive.Default();
        }
        _archives.Add(fullName, archive);
        return archive;
    }
    public void SaveArchive(IArchive archive)
    {
        PlayerPrefs.SetString(archive.GetType().FullName, EncryptUtil.DESEncrypt(JsonMapper.ToJson(archive), _encryptKey));
        PlayerPrefs.Save();
    }
    public void SaveAll()
    {
        foreach(var archive in _archives.Values) {
            SaveArchive(archive);
        }
        PlayerPrefs.Save();
    }
    public void ClearAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

#if UNITY_EDITOR
    // TODO: debug mode in mobile
    [UnityEditor.MenuItem("Dev/ClearAll")]
    public static void SetPlayMoney_Menu()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

#endif
}