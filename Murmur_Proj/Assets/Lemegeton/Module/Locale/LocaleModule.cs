using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Lemegeton
{
    public static class LocaleType
    {
        public static readonly string English = "en";
        public static readonly string Japanese = "jp";
        public static readonly string TChinese = "tw";
        public static readonly string Spainish = "es";
        public static readonly string German = "de";
        public static readonly string French = "fr";
    }
    public class LocaleModule : Singleton<LocaleModule>
    {
        protected LocaleModule() { }
        private bool inited = false;
        private Dictionary<string, string> _localeTexts;
        private  Dictionary<SystemLanguage, string> _locales = new Dictionary<SystemLanguage, string>()
        {
            {SystemLanguage.English, "en"},
            {SystemLanguage.ChineseTraditional, "tw"},
            {SystemLanguage.ChineseSimplified, "tw"},
            {SystemLanguage.Chinese, "tw"},
            {SystemLanguage.Japanese, "jp"},
            {SystemLanguage.Spanish, "es"},
            {SystemLanguage.German, "de"},
            {SystemLanguage.French, "fr"},
        };
        public string locale;

        public async Task Init()
        {
            if(inited) return;

            var lang = Application.systemLanguage;
            if(!_locales.ContainsKey(lang)) locale = LocaleType.English;
            else locale = _locales[lang];

            _localeTexts = new Dictionary<string, string>();
            await LoadAsset();
        }
        public async Task LoadAsset()
        {
            var handleData = Addressables.LoadAssetAsync<TextAsset>(string.Format(GameUtil.ResLocalePath, locale));
            await handleData.Task;
            if (handleData.Status == AsyncOperationStatus.Succeeded) {
                var jsonStr = EncryptUtil.DESDecrypt(handleData.Result.text, GameUtil.ResLocalCryptKey);
                _localeTexts = LitJson.JsonMapper.ToObject<Dictionary<string, string>>(jsonStr);
                inited = true;
                Addressables.Release(handleData);
            } else {
                Debug.LogError("failed to load config data ");
            }
        }
        public string GetLocale(string key)
        {
            if(_localeTexts.TryGetValue(key, out var localText)) {
                return localText;
            } else {
                Debug.LogWarning("[LocaleModule] GetLocale : " + locale + " text Not Found. key = " + key);
                return locale + " - " + key;
            }
        }

        public string GetLocale(string key, params object[] args)
        {
            string ret = GetLocale(key);
            return string.Format(ret, args);
        }
    }
    public static class StringExtension
    {
        public static string Locale(this string key)
        {
            return LocaleModule.Instance.GetLocale(key);
        }

        public static string Locale(this string key, params object[] args)
        {
            return LocaleModule.Instance.GetLocale(key, args);
        }
    }
}

