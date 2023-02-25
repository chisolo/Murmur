using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Lemegeton
{
    public class PrefabLoader
    {
        public static AsyncOperationHandle<GameObject> InstantiateAsync(string address, Transform parent)
        {
            var handle = Addressables.InstantiateAsync(address, parent);
            handle.Completed += InstantiateCompleted;
            return handle;
        }

        public static AsyncOperationHandle<GameObject> InstantiateAsync(string address, Vector3 position, Quaternion rotation, Transform parent)
        {
            var handle = Addressables.InstantiateAsync(address, position, rotation, parent);
            handle.Completed += InstantiateCompleted;
            return handle;
        }

        public static AsyncOperationHandle<GameObject> InstantiateAsync(string address, Transform parent, Action<GameObject> onComplete)
        {
            var handle = Addressables.InstantiateAsync(address, parent);
            handle.Completed += (op => {
                InstantiateCompleted(op);
                onComplete?.Invoke(op.Result);
            });
            return handle;
        }

        public static AsyncOperationHandle<GameObject> InstantiateAsync(string address, Vector3 position, Quaternion rotation, Transform parent, Action<GameObject> onComplete = null)
        {
            var handle = Addressables.InstantiateAsync(address, position, rotation, parent);
            handle.Completed += (op => {
                InstantiateCompleted(op);
                onComplete?.Invoke(op.Result);
            });
            return handle;
        }

        public static AsyncOperationHandle<GameObject> InstantiateAsync<T>(string address, Transform parent, Action<T> onComplete = null)
        {
            var handle = Addressables.InstantiateAsync(address, parent);
            handle.Completed += (op => {
                InstantiateCompleted(op);
                var component = op.Result.GetComponent<T>();
                onComplete?.Invoke(component);
            });
            return handle;
        }

        public static AsyncOperationHandle<GameObject> InstantiateAsync<T>(string address, Vector3 position, Quaternion rotation, Transform parent, Action<T> onComplete = null)
        {
            var handle = Addressables.InstantiateAsync(address, position, rotation, parent);
            handle.Completed += (op => {
                InstantiateCompleted(op);
                var component = op.Result.GetComponent<T>();
                onComplete?.Invoke(component);
            });
            return handle;
        }
        public static async Task<GameObject> InstantiateAsyncTask(string address, Vector3 position, Quaternion rotation, Transform parent)
        {
            var handle = Addressables.InstantiateAsync(address, position, rotation, parent);
            handle.Completed += InstantiateCompleted;
            await handle.Task;
            return handle.Result;
        }
        public static async Task<GameObject> InstantiateAsyncTask(string address, Transform parent)
        {
            var handle = Addressables.InstantiateAsync(address, parent);
            handle.Completed += InstantiateCompleted;
            await handle.Task;
            return handle.Result;
        }
        public static async Task<T> InstantiateAsyncTask<T>(string address, Transform parent)
        {
            var handle = Addressables.InstantiateAsync(address, parent);
            handle.Completed += InstantiateCompleted;
            await handle.Task;
            
            var component = handle.Result.GetComponent<T>();
            return component;
        }

        private static void InstantiateCompleted(AsyncOperationHandle<GameObject> obj) {
            // Add component to release asset in GameObject OnDestroy event
            obj.Result.AddComponent(typeof(SelfCleanup));
        }

    }

    // Releases asset (trackHandle must be true in InstantiateAsync,
    // which is the default)
    public class SelfCleanup : MonoBehaviour
    {
        void OnDestroy() {
            Addressables.ReleaseInstance(gameObject);
        }
    }
}
