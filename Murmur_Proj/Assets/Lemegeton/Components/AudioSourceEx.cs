using UnityEngine;

namespace Lemegeton.AudioExtension
{
    public static class AudioSourceEx
    {
        public static void Play(this AudioSource source, string address)
        {
            ResourceModule.Instance.LoadAndPlayAudioClip(source, address);
        }


    }
}