using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Plays one track at a time; switching tracks crossfades between two alternating sources.
/// </summary>
public static class Music
{
    private const string FOLDER = "Audio/Music/";

    private static readonly Dictionary<string, AudioClip> _clips = new();
    private static AudioSource _active;
    private static AudioSource _inactive;
    private static bool _isMuted;

    public static string CurrentKey { get; private set; }
    public static bool IsPlaying => CurrentKey != null && _active.isPlaying;

    public static bool IsMuted
    {
        get => _isMuted;
        set
        {
            _isMuted = value;
            _active.mute = value;
            _inactive.mute = value;
        }
    }

    internal static void Init()
    {
        _clips.Clear();
        _active = AudioRoot.CreateSource(AudioRoot.MusicGroup);
        _inactive = AudioRoot.CreateSource(AudioRoot.MusicGroup);
        CurrentKey = null;
    }

    public static void Play(string key, float crossfade = 1f, float volume = 1f, bool loop = true)
    {
        if (key == CurrentKey && _active.isPlaying) return;

        var clip = AudioRoot.LoadClip(FOLDER, key, _clips);
        if (clip == null) return;

        (_active, _inactive) = (_inactive, _active);
        FadeOutAndStop(_inactive, crossfade);

        _active.DOKill();
        _active.clip = clip;
        _active.loop = loop;
        _active.mute = _isMuted;
        _active.volume = crossfade > 0f ? 0f : volume;
        _active.Play();
        if (crossfade > 0f) _active.DOFade(volume, crossfade);

        CurrentKey = key;
    }

    public static void Stop(float fadeOut = 1f)
    {
        FadeOutAndStop(_active, fadeOut);
        CurrentKey = null;
    }

    public static void SetVolume(float volume, float duration = 0f)
    {
        _active.DOKill();
        if (duration <= 0f)
        {
            _active.volume = volume;
            return;
        }
        _active.DOFade(volume, duration);
    }

    private static void FadeOutAndStop(AudioSource source, float duration)
    {
        source.DOKill();
        if (duration <= 0f || !source.isPlaying)
        {
            source.Stop();
            return;
        }
        source.DOFade(0f, duration).OnComplete(source.Stop);
    }
}
