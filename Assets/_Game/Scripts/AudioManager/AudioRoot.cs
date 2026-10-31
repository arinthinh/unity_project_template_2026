using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

internal static class AudioRoot
{
    private const string MIXER_PATH = "Audio/Mixer";
    private const string MUSIC_GROUP = "Music";
    private const string SFX_GROUP = "Sound";

    public static GameObject Host { get; private set; }
    public static AudioMixerGroup MusicGroup { get; private set; }
    public static AudioMixerGroup SfxGroup { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        Host = new GameObject("[Audio]");
        Object.DontDestroyOnLoad(Host);

        var mixer = Resources.Load<AudioMixer>(MIXER_PATH);
        if (mixer == null)
        {
            Debug.LogWarning($"[Audio] Mixer not found at Resources/{MIXER_PATH}. Audio will play without mixer routing.");
        }
        else
        {
            MusicGroup = FindGroup(mixer, MUSIC_GROUP);
            SfxGroup = FindGroup(mixer, SFX_GROUP);
        }

        Sfx.Init();
        Music.Init();
    }

    public static AudioSource CreateSource(AudioMixerGroup group)
    {
        var source = Host.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.outputAudioMixerGroup = group;
        return source;
    }

    public static AudioClip LoadClip(string folder, string key, Dictionary<string, AudioClip> cache)
    {
        if (cache.TryGetValue(key, out var clip)) return clip;

        clip = Resources.Load<AudioClip>(folder + key);
        if (clip == null)
        {
            Debug.LogError($"[Audio] Clip not found: Resources/{folder}{key}");
            return null;
        }

        cache[key] = clip;
        return clip;
    }

    private static AudioMixerGroup FindGroup(AudioMixer mixer, string name)
    {
        var groups = mixer.FindMatchingGroups(name);
        if (groups.Length > 0) return groups[0];
        Debug.LogWarning($"[Audio] Mixer group '{name}' not found.");
        return null;
    }
}
