using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace Lemegeton
{
    public class ParticleSystemEffect: EffectAsset
    {
        public class StoppedScript: MonoBehaviour
        {
            public System.Action onParticleStop;
            public void OnParticleSystemStopped()
            {
                onParticleStop?.Invoke();
            }
        }

        [SerializeField] ParticleSystem _ps;

        void Awake()
        {
            //Debug.Log("awak");
            Setup();
        }

        // void Start()
        // {
        //     //Debug.Log("Start");
        // }

        // void OnEnable()
        // {
        //     //Debug.Log("OnEnable");
        // }
        // void OnDisable()
        // {
        //     //Debug.Log("OnDisable");
        // }

        public void Setup()
        {
            var main = _ps.main;
            main.loop = false;
            main.stopAction = ParticleSystemStopAction.Callback;

            var stop = _ps.gameObject.AddComponent<StoppedScript>();
            stop.onParticleStop = () => {
                this.onStop?.Invoke();
            };
        }
    }
}