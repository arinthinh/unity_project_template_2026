using UnityEngine;

namespace Framework
{
    public static class AudioHelper
    {
        public static void PlayRandomSound(ESound[] sounds) => AudioManager.Instance.PlaySound(sounds[Random.Range(0, sounds.Length)]);

        public static void PlaySoundIfNotPlaying(ESound soundKey)
        {
            if (AudioManager.Instance.IsSoundPlaying(soundKey)) return;
            AudioManager.Instance.PlaySound(soundKey);
        }

        public static void PlaySound(ESound soundKey, float volume = 1, float delay = 0, bool isLoop = false, bool isSetTimeStop = false,
            float timeStop = 0)
        {
            AudioManager.Instance.PlaySound(soundKey, volume, delay, isLoop, isSetTimeStop, timeStop);
        }

        public static void StopSound(ESound soundKey)
        {
            AudioManager.Instance.StopSound(soundKey);
        }

        public static void StopMusic(EMusic soundKey)
        {
            AudioManager.Instance.StopMusic(soundKey);
        }

        public static void PlayMusic(EMusic key)
        {
            AudioManager.Instance.PlayMusic(key, 1);
        }
    }
}