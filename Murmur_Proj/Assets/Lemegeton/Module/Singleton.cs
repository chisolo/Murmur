using UnityEngine;

namespace Lemegeton
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static bool _applicationIsQutting = false;
        private static object _lock = new object();
        public static T Instance {
            get {
                if(_applicationIsQutting) {
                    return null;
                }
                lock(_lock) {
                    if(_instance == null) {
                        _instance = (T)FindObjectOfType(typeof(T));
                        if(FindObjectsOfType(typeof(T)).Length > 1) {
                            return _instance;
                        }
                        if(_instance == null) {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "Singleton_" + typeof(T).ToString();
                            DontDestroyOnLoad(singleton);
                        } else {
                            Debug.LogError("[Singleton] Using instance already created: " + _instance.gameObject.name);
                        }
                    }
                    return _instance;
                }
            }

        }
        void OnApplicationQuit()
        {
            _applicationIsQutting = true;
        }
        public void OnDestroy()
        {
            _applicationIsQutting = true;
        }
    }
}