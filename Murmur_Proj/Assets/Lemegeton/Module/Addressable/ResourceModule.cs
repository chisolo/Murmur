using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityEngine.U2D;

namespace Lemegeton
{
    public class ResourceModule: Singleton<ResourceModule>
    {
        public void LoadSpriteAsync(Image img, string address)
        {
            if (string.IsNullOrEmpty(address)) {
                Debug.LogError("address is invalid");
                return;
            }

            img.color = Color.clear;
            var handle = Addressables.LoadAssetAsync<Sprite>(address);
            handle.Completed += (op) => {
                img.sprite = handle.Result;
                img.color = Color.white;
                ReferenceModule.Instance.AddComponentRef(handle, img);
            };
        }

        public void LoadAtlasedSpriteAsync(Image img, string address, string spriteName)
        {
            if (string.IsNullOrEmpty(address)) {
                Debug.LogError("address is invalid");
                return;
            }

            img.color = Color.clear;
            var handle = Addressables.LoadAssetAsync<SpriteAtlas>(address);
            handle.Completed += (op) => {
                img.sprite = handle.Result.GetSprite(spriteName);
                img.color = Color.white;
                ReferenceModule.Instance.AddComponentRef(handle, img);
            };
        }

        public void LoadAndPlayAudioClip(AudioSource source, string address)
        {
            if (string.IsNullOrEmpty(address)) {
                Debug.LogError("address is invalid");
                return;
            }

            var clip = ReferenceModule.Instance.GetAudioClip(address);
            if (clip != null) {
                source.clip = clip;
                source.Play();
                ReferenceModule.Instance.UpdateAudioRef(address, source);
                return;
            }

            var handle = Addressables.LoadAssetAsync<AudioClip>(address);
            handle.Completed += (op) => {
                source.clip = handle.Result;
                source.Play();
                ReferenceModule.Instance.AddAudioRef(handle, address, source);
            };
        }

    }
}