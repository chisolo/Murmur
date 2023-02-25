using System;
using UnityEngine;
using UnityEngine.UI;


using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

namespace App.UI
{
    [Obsolete]
    [RequireComponent(typeof(Image))]
    public class AddressableImageOld : MonoBehaviour
    {

        [SerializeField]
        private string imageAddress;
        private AsyncOperationHandle _opHandle;
        private AsyncOperationHandle<Sprite> _lodingHandle;
        private Task _t = null;


        private Image _imgObj;

        protected void Start()
        {
            _imgObj = GetComponent<Image>();
            if (!string.IsNullOrEmpty(imageAddress)) {
                LoadSprite(this.imageAddress);
            }
        }

        public void LoadSprite(string address)
        {
            if (string.IsNullOrEmpty(address)) {
                Debug.LogError("address is invalid");
                return;
            }

            LoadSpriteAync(address);
        }

        async private void LoadSpriteAync(string address)
        {
            _t = LoadSpriteTask(address);
            await _t;
        }

        async private Task LoadSpriteTask(string address)
        {
            if (_t != null && !_t.IsCompleted) {
               await _t;
            }


            Debug.Log("load start");


            _lodingHandle = Addressables.LoadAssetAsync<Sprite>(address);

            await _lodingHandle.Task;

            if (_lodingHandle.Status == AsyncOperationStatus.Succeeded) {

                _imgObj.sprite = _lodingHandle.Result;

                //Debug.Log("frame " + Time.frameCount);
                //Debug.Log("frame " + Time.frameCount);

                // release old sprite
                if (this._opHandle.IsValid()) {
                    Addressables.Release(this._opHandle);
                }

                this.imageAddress = address;
                this._opHandle = _lodingHandle;
            } else {
                Debug.LogError("load sprite failed " + address);
            }


            Debug.Log("load end");
        }

        public void UnloadSprite()
        {
            if (this._opHandle.IsValid()) {
                Addressables.Release(this._opHandle);
                _imgObj.sprite = null;
                this.imageAddress = null;
            }

            if (this._lodingHandle.IsValid()) {
                Addressables.Release(this._lodingHandle);
            }
        }

        protected void OnDestroy()
        {
            Debug.Log("OnDestroy");
            UnloadSprite();
        }
    }
}