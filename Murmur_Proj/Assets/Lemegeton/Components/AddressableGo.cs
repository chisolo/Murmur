using System;
using UnityEngine;

namespace Lemegeton
{
    public class AddressableGo : MonoBehaviour
    {
        public string address;
        private void Awake()
        {
            if (!string.IsNullOrEmpty(address)) {
                PrefabLoader.InstantiateAsync(address, transform);
            }
        }
    }
}