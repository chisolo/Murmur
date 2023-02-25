using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Pool;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace Lemegeton
{
    public class AddressablePoolModule: Singleton<AddressablePoolModule>
    {
        private Transform poolRoot;

        Dictionary<string, AsyncOperationHandle<GameObject>> assetHandleList = new Dictionary<string, AsyncOperationHandle<GameObject>>();
        Dictionary<string, AssetPool> poolList = new Dictionary<string, AssetPool>();
        public bool collectionChecks = true;
        public int poolCapacity = 10;
        public int maxPoolSize = 10;

        void Awake()
        {
            var go = new GameObject("pool");
            go.transform.parent = transform;
            poolRoot = go.transform;
        }

        public async void Prepare(string address, Action onComplete = null)
        {
            if (assetHandleList.ContainsKey(address)) {
                return;
            }

            var handle = Addressables.LoadAssetAsync<GameObject>(address);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded) {
                assetHandleList.Add(address, handle);
                onComplete?.Invoke();
            } else {
                Debug.LogError("load asset failed " + address);
            }
        }


        // public async void LoadAndGetAsync(string address, Action<GameObject> onComplete)
        // {
        //     if (assetHandleList.ContainsKey(address)) {
        //         var go = Get(address);
        //         onComplete(go);
        //         return;
        //     }

        //     var handle = Addressables.LoadAssetAsync<GameObject>(address);
        //     await handle.Task;

        //     if (handle.Status == AsyncOperationStatus.Succeeded) {
        //         assetHandleList.Add(address, handle);

        //         var go = Get(address);
        //         onComplete(go);
        //     } else {
        //         Debug.LogError("load asset failed " + address);
        //         onComplete(null);
        //     }
        // }

        public async Task<GameObject> LoadAndGetAsyncTask(string address)
        {
            if (assetHandleList.ContainsKey(address)) {
                while(!poolList.ContainsKey(address)) {
                    await Task.Yield();
                }
                var go = Get(address);
                return go;
            }

            var handle = Addressables.LoadAssetAsync<GameObject>(address);
            assetHandleList.Add(address, handle);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded) {
                var go = Get(address);
                return go;
            }
            Debug.LogError("load asset failed " + address);
            return null;
        }
        public async Task<T> LoadAndGetNoCache<T>(string address) where T : UnityEngine.Object
        {

            var handle = Addressables.LoadAssetAsync<T>(address);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded) {
                return handle.Result as T;
            }
            Debug.LogError("load asset failed " + address);
            return null;
        }
        public GameObject Get(string address)
        {
            if (!assetHandleList.ContainsKey(address)) {
                Debug.LogError("asset has not be loaded");
                return null;
            }

            if (!poolList.ContainsKey(address)) {
                poolList[address] = new AssetPool(assetHandleList[address].Result, collectionChecks, poolCapacity, maxPoolSize);
            }

            var go = poolList[address].Get();

            return go;
        }

        public void Return(string address, GameObject go)
        {
            if (!poolList.ContainsKey(address)) {
                Debug.LogError("no item");
                return;
            }

            poolList[address].Release(go);
        }

        public void Clear(string address)
        {
            if (assetHandleList.ContainsKey(address)) {
                var handle = assetHandleList[address];
                Addressables.Release(handle);

                assetHandleList.Remove(address);
            }

            if (poolList.ContainsKey(address)) {
                poolList[address].Clear();
                poolList.Remove(address);
            }
        }

    }


    class AssetPool
    {
        private ObjectPool<GameObject> innerPool;
        private GameObject prefab;



        public AssetPool(GameObject prefab, bool collectionChecks = true, int poolCapacity = 10, int maxPoolSize = 10)
        {
            this.prefab = prefab;
            innerPool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, poolCapacity, maxPoolSize);
        }

        public GameObject Get()
        {
            return innerPool.Get();
        }

        public void Release(GameObject go)
        {
            innerPool.Release(go);
        }

        public void Clear()
        {
            innerPool.Clear();
        }

        private GameObject CreatePooledItem()
        {
            //Debug.Log("CreatePooledItem");
            var go = GameObject.Instantiate(prefab);

            // TODO: create and use pool root
            //go.transform.parent = poolRoot;
            return go;
        }

        private void OnReturnedToPool(GameObject go)
        {
            //Debug.Log("OnReturnedToPool");
            go.SetActive(false);
            //go.transform.parent = poolRoot;
        }

        private void OnTakeFromPool(GameObject go)
        {
            //Debug.Log("OnTakeFromPool");
            go.SetActive(true);
        }

        private void OnDestroyPoolObject(GameObject go)
        {
            //Debug.Log("OnDestroyPoolObject");
            GameObject.Destroy(go);
        }
    }
}
