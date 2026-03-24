using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Framework
{
    public class AudioChannel<T>
    {
        public string ChannelId;
        /// <summary>
        /// When ever play a music from this channel, this id will be set
        /// </summary>
        public string PlayKey;
        public T AudioKey;
        public bool IsPlay;
        public AudioSource Source;

        /// <summary>
        /// Mark it as borrowed, which means it will not be used by the Audio Manager
        /// </summary>
        public bool IsBorrow = false;
        
        private Tween _autoStopTween;

        public bool IsMute
        {
            get => Source.mute;
            set => Source.mute = value;
        }

        public AudioChannel(AudioSource source)
        {
            ChannelId = FriendlyGUID.NewId_FromRandomInt();
            Source = source;
        }

        public void Play(T audioKey, AudioClip clip, float volume, bool isMute, bool isLoop = true, bool isSetTimeStop = false, float stopAfter = 0,
            float delay = 0)
        {
            PlayKey = FriendlyGUID.NewId_FromRandomInt();

            IsPlay = true;
            AudioKey = audioKey;
            Source.clip = clip;
            Source.loop = isLoop;
            Source.mute = isMute;
            Source.volume = volume;
            Source.PlayDelayed(delay);

            if (isSetTimeStop)
            {
                _autoStopTween = DOVirtual.DelayedCall(delay + stopAfter, Stop);
            }
            else if (!isLoop)
            {
                _autoStopTween = DOVirtual.DelayedCall(clip.length, Stop);
            }
        }

        public void Stop()
        {
            IsPlay = false;
            Source.Stop();
            _autoStopTween?.Kill();
        }
    }
}