using UnityEngine;

namespace Lemegeton
{
    public static class GameObjectEx
    {
        public static void SetActiveIfNeed(this GameObject go, bool active)
        {
            if (go.activeSelf != active) {
                go.SetActive(active);
            }
        }
    }
}