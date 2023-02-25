using UnityEngine;


public abstract class GameMgr<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;
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
                    if(_instance == null) {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "GameMgr_" + typeof(T).ToString();
                        //DontDestroyOnLoad(singleton);
                    } else {
                        return _instance;
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
    public virtual void OnDestroy()
    {
        _applicationIsQutting = true;
    }
}
