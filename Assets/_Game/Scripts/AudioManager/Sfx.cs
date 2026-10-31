using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public static class Sfx
{
    private const string FOLDER = "Audio/Sfx/";
    private const int MAX_VOICES = 32;

    private static readonly Dictionary<string, AudioClip> _clips = new();
    private static readonly List<SfxVoice> _voices = new();
    private static bool _isMuted;

    public static bool IsMuted
    {
        get => _isMuted;
        set
        {
            _isMuted = value;
            foreach (var voice in _voices) voice.Source.mute = value;
        }
    }

    internal static void Init()
    {
        _clips.Clear();
        _voices.Clear();
    }

    public static SfxHandle Play(string key, float volume = 1f, float pitch = 1f, float randomPitch = 0f, float delay = 0f, bool loop = false)
    {
        var clip = AudioRoot.LoadClip(FOLDER, key, _clips);
        if (clip == null) return default;

        var voice = GetFreeVoice();
        if (voice == null)
        {
            Debug.LogWarning($"[Audio] Max {MAX_VOICES} sfx voices reached, skip '{key}'.");
            return default;
        }

        var source = voice.Source;
        source.DOKill();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch + Random.Range(-randomPitch, randomPitch);
        source.loop = loop;
        source.mute = _isMuted;
        source.PlayDelayed(delay);

        voice.Key = key;
        voice.Version++;
        return new SfxHandle(voice);
    }

    public static bool IsPlaying(string key) => _voices.Exists(v => v.Key == key && v.Source.isPlaying);

    public static void Stop(string key, float fadeOut = 0f)
    {
        foreach (var voice in _voices)
        {
            if (voice.Key == key && voice.Source.isPlaying) voice.Stop(fadeOut);
        }
    }

    public static void StopAll(float fadeOut = 0f)
    {
        foreach (var voice in _voices) voice.Stop(fadeOut);
    }

    private static SfxVoice GetFreeVoice()
    {
        var voice = _voices.Find(v => !v.Source.isPlaying);
        if (voice != null) return voice;
        if (_voices.Count >= MAX_VOICES) return null;

        voice = new(AudioRoot.CreateSource(AudioRoot.SfxGroup));
        _voices.Add(voice);
        return voice;
    }
}

internal class SfxVoice
{
    public readonly AudioSource Source;
    public string Key;
    public int Version;

    public SfxVoice(AudioSource source) => Source = source;

    public void Stop(float fadeOut)
    {
        Source.DOKill();
        if (fadeOut <= 0f)
        {
            Source.Stop();
            return;
        }
        Source.DOFade(0f, fadeOut).OnComplete(Source.Stop);
    }
}

/// <summary>
/// Becomes invalid once its voice is reused by another Play call, so holding an old handle never affects a different sound.
/// </summary>
public readonly struct SfxHandle
{
    private readonly SfxVoice _voice;
    private readonly int _version;

    internal SfxHandle(SfxVoice voice)
    {
        _voice = voice;
        _version = voice.Version;
    }

    public bool IsValid => _voice != null && _voice.Version == _version;
    public bool IsPlaying => IsValid && _voice.Source.isPlaying;

    public void Stop(float fadeOut = 0f)
    {
        if (!IsValid) return;
        _voice.Stop(fadeOut);
    }

    public void SetVolume(float volume)
    {
        if (!IsValid) return;
        _voice.Source.volume = volume;
    }

    public void SetPitch(float pitch)
    {
        if (!IsValid) return;
        _voice.Source.pitch = pitch;
    }
}
