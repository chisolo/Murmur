using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace Lemegeton
{
    public class AudioModule : Singleton<AudioModule> 
    {
        protected AudioModule() { }
        public string identify = "AudioModule";
        private readonly int MaxSourceCount = 10;
        private AudioSource _bgmSource;
        private AudioClip _bgmClip;
        private List<AudioSource> _sfxSources;
        private List<AudioSource> _prefabSources;
        private Dictionary<string, AudioClip> _sfxs = new Dictionary<string, AudioClip>();
        private bool _sfxMute;
        private bool isInited = false;
        public void Init()
        {
            if(isInited) return;
            _bgmSource = gameObject.AddComponent<AudioSource>();
            _sfxSources = new List<AudioSource>();
            _prefabSources = new List<AudioSource>();
            _sfxs = new Dictionary<string, AudioClip>();
            isInited = true;
            gameObject.AddComponent<AudioListener>();
        }

        public async void PlayBgm(string bgm)
        {
            if(_bgmClip == null) {
                _bgmClip = await AddressablePoolModule.Instance.LoadAndGetNoCache<AudioClip>(bgm);

            }
            _bgmSource.clip = _bgmClip;
            _bgmSource.loop = true;
            _bgmSource.volume = 0.3f;
            _bgmSource.Play();
        }

        public async void PlaySfx(string sfx, bool isloop = false)
        {
            if(!_sfxs.ContainsKey(sfx)) {
                var audioClip = await AddressablePoolModule.Instance.LoadAndGetNoCache<AudioClip>(sfx);
                _sfxs.Add(sfx, audioClip);
            }

            AudioSource source = _sfxSources.FirstOrDefault(s => !s.isPlaying);
            if(source == null) {
                if(_sfxSources.Count >= MaxSourceCount) {
                    Debug.LogWarning("[AudioModule] Audio Source is full !");
                    return;
                }
                source = gameObject.AddComponent<AudioSource>();
                _sfxSources.Add(source);
                source.mute = _sfxMute;
            }
            
            source.clip = _sfxs[sfx];
            source.loop = isloop;
            source.Play();
        }

        public void StopSfx()
        {
            foreach(var source in _sfxSources) {
                if(source.loop == true) {
                    source.Stop();
                }
            }
            _bgmSource.Stop();
        }

        public void UpdateMute(bool bgmMute, bool sfxMute)
        {
            _sfxMute = sfxMute;
            _bgmSource.mute = bgmMute;
            foreach (var audioSource in _sfxSources)
            {
                audioSource.mute = sfxMute;
            }
        }
    }
}

