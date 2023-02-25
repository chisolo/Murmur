using UnityEngine;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace Lemegeton
{
    public interface IReference
    {
        void Release();

        bool IsVaild();

        void Update(float deltaTime);
    }

    /// <summary>
    /// addressable引用
    /// 当Component为空时，释放资源
    /// </summary>
    public class ComponentReference : IReference
    {
        public AsyncOperationHandle Target;
        public Component Dependency;

        public ComponentReference(AsyncOperationHandle target, Component dependency)
        {
            Target = target;
            Dependency = dependency;
        }

        public void Update(float deltaTime) {}

        public bool IsVaild()
        {
            return Dependency != null;
        }

        public void Release()
        {
            Addressables.Release(Target);
        }
    }

    public class AudioClipReference : IReference
    {
        public string Address;
        public AsyncOperationHandle Target;
        public AudioSource Source;
        public AudioClip Clip;

        public float time = 0f;

        public AudioClipReference(AsyncOperationHandle<AudioClip> target, string address, AudioSource source)
        {
            Address = address;
            Target = target;
            Source = source;
            Clip = target.Result;
        }

        public void Update(float deltaTime)
        {
            if (Source != null && Source.isPlaying) {
                time = 0;
            } else {
                time += deltaTime;

                if (Source != null) Source.clip = null;
                Source = null;
            }
        }

        public void ReUse(AudioSource source)
        {
            Source = source;
            time = 0f;
        }

        public bool IsVaild()
        {
            return time < 60f;
        }

        public void Release()
        {
            Addressables.Release(Target);
        }
    }


    /// <summary>
    /// 引用管理
    /// </summary>
    public class ReferenceModule: Singleton<ReferenceModule>
    {
        private List<IReference> _refList = new List<IReference>();

        private float _deltaTime = 0;
        public void AddComponentRef(AsyncOperationHandle target, Component refby)
        {
            var refObj = new ComponentReference(target, refby);

            _refList.Add(refObj);
        }

        public AudioClip GetAudioClip(string address)
        {
            foreach (var reference in _refList) {
                if (reference is AudioClipReference) {
                    var audioRef = reference as AudioClipReference;
                    if (audioRef.Address == address) {
                        return audioRef.Clip;
                    }
                }
            }

            return null;
        }

        public void AddAudioRef(AsyncOperationHandle<AudioClip> target, string address, AudioSource source)
        {
            var refObj = new AudioClipReference(target, address, source);

            _refList.Add(refObj);
        }

        public void UpdateAudioRef(string address, AudioSource source)
        {
            foreach (var reference in _refList) {
                if (reference is AudioClipReference) {
                    var audioRef = reference as AudioClipReference;
                    if (audioRef.Address == address) {
                        audioRef.ReUse(source);
                        return;
                    }
                }
            }
            Debug.LogError("no such audio clip " + address);
        }

        private void Update()
        {
            _deltaTime += Time.deltaTime;
            if (_deltaTime > 1) {
                for (int i = _refList.Count - 1; i >= 0; i--) {
                    var refObj = _refList[i];
                    refObj.Update(_deltaTime);

                    if (!refObj.IsVaild()) {
                        refObj.Release();
                        _refList.RemoveAt(i);
                    }
                }

                _deltaTime = 0f;
            }
        }
    }
}