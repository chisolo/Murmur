using UnityEngine;
using UnityEngine.UI;

namespace Lemegeton
{
    public static class ImageEx
    {
        public static void ShowSprite(this Image img, string address)
        {
            ResourceModule.Instance.LoadSpriteAsync(img, address);
        }

        [System.Obsolete]
        public static void ShowAtlasedSprite(this Image img, string address, string spriteName)
        {
            ResourceModule.Instance.LoadAtlasedSpriteAsync(img, address, spriteName);
        }

    }
}