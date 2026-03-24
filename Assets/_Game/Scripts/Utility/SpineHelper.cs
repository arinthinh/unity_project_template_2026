using System;
using Spine;
using Spine.Unity;

namespace SpineHelper
{
    public static class SpineHelper
    {
        #region SKELETON ANIMATION

        public static TrackEntry SetAnimation(this SkeletonAnimation skeletonAnimation, string animationName, bool loop = false)
        {
            return skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);
        }

        public static TrackEntry AddAnimation(this SkeletonAnimation skeletonAnimation, string animationName, bool loop = false)
        {
            return skeletonAnimation.AnimationState.AddAnimation(0, animationName, loop, 0);
        }

        public static void SetSkin(this SkeletonAnimation skeletonAnimation, string skinName)
        {
            skeletonAnimation.Skeleton.SetSkin(skinName);
            skeletonAnimation.Skeleton.SetSlotsToSetupPose();
        }

        public static float GetAnimationDuration(this SkeletonAnimation skeletonAnimation, string animationName)
        {
            var animGet = skeletonAnimation.Skeleton.Data.FindAnimation(animationName);
            return animGet?.Duration ?? 0;
        }

        #endregion

        #region SKELETON GRAPHIC

        public static TrackEntry SetAnimation(this SkeletonGraphic skeletonAnimation, string animationName, bool loop = false)
        {
            return skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);
        }

        public static TrackEntry AddAnimation(this SkeletonGraphic skeletonAnimation, string animationName, bool loop = false)
        {
            return skeletonAnimation.AnimationState.AddAnimation(0, animationName, loop, 0);
        }

        public static TrackEntry OnComplete(this TrackEntry trackEntry, Action onComplete)
        {
            trackEntry.Complete += t => onComplete?.Invoke();
            return trackEntry;
        }

        public static TrackEntry OnSoundEvent(this TrackEntry trackEntry, Action soundPlayInvoke)
        {
            trackEntry.Event += OnEvent;

            return trackEntry;

            void OnEvent(TrackEntry t, Event e)
            {
                if (e.Data.Name == "sound")
                {
                    soundPlayInvoke?.Invoke();
                }
            }
        }

        public static TrackEntry OnEvent(this TrackEntry trackEntry, Action<string> eventInvoke)
        {
            trackEntry.Event += (t, e) => { eventInvoke?.Invoke(e.Data.Name); };
            return trackEntry;
        }

        public static void SetSkin(this SkeletonGraphic skeletonAnimation, string skinName)
        {
            skeletonAnimation.Skeleton.SetSkin(skinName);
            skeletonAnimation.Skeleton.SetSlotsToSetupPose();
        }

        public static float GetAnimationDuration(this SkeletonGraphic skeletonAnimation, string animationName)
        {
            var animGet = skeletonAnimation.Skeleton.Data.FindAnimation(animationName);
            return animGet?.Duration ?? 0;
        }

        public static float Pause(this SkeletonGraphic skeletonGraphic)
        {
            skeletonGraphic.AnimationState.Tracks.Items[0].TimeScale = 0;
            return skeletonGraphic.AnimationState.Tracks.Items[0].TrackTime;
        }

        public static float Resume(this SkeletonGraphic skeletonGraphic, float time)
        {
            skeletonGraphic.AnimationState.Tracks.Items[0].TrackTime = time;
            skeletonGraphic.AnimationState.Tracks.Items[0].TimeScale = 1;
            return skeletonGraphic.AnimationState.Tracks.Items[0].TrackTime;
        }

        public static float Resume(this SkeletonGraphic skeletonGraphic) =>
            Resume(skeletonGraphic, skeletonGraphic.AnimationState.Tracks.Items[0].TrackTime);

        public static bool IsPlayingAnimation(this SkeletonGraphic skeletonGraphic, string animationName)
        {
            if (skeletonGraphic.AnimationState.Tracks.Count <= 0) return false;
            return skeletonGraphic.AnimationState.Tracks.Items[0].Animation.Name == animationName;
        }

        #endregion
    }
}