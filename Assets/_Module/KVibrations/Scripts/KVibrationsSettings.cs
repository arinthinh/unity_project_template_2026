using UnityEngine;
using System;

namespace Amanotes.Orchestra
{
    /// <summary>
    /// Enum that represents all the types of haptic patterns available
    /// </summary>
    [Serializable]
    public enum VibrationType
    {
        Selection = 0,
        Success = 1,
        Warning = 2,
        Failure = 3,
        LightImpact = 4,
        MediumImpact = 5,
        HeavyImpact = 6,
        RigidImpact = 7,
        SoftImpact = 8,
        None = -1
    }

    /// <summary>
    /// Represents a vibration pattern with time and amplitude data
    /// </summary>
    [Serializable]
    public class VibrationPattern
    {
        public VibrationType VibrationType;
        public float[] time;
        public float[] amplitude;

        public VibrationPattern(VibrationType type, float[] time, float[] amplitude)
        {
            this.VibrationType = type;
            this.time = time;
            this.amplitude = amplitude;
        }

        public float GetDuration()
        {
            if (time != null && time.Length > 0)
            {
                return time[time.Length - 1];
            }
            return 0f;
        }
    }

    /// <summary>
    /// ScriptableObject that holds all vibration pattern configurations
    /// Patterns are separated for iOS and Android platforms
    /// </summary>
    [CreateAssetMenu(fileName = "KVibrationsSettings", menuName = "KVibrations/Settings", order = 1)]
    public class KVibrationsSettings : ScriptableObject
    {
        [Header("iOS Patterns")]
        [SerializeField]
        public VibrationPattern[] iOSPatterns;

        [Header("Android Patterns")]
        [SerializeField]
        public VibrationPattern[] androidPatterns;

        private void OnEnable()
        {
            // Initialize with default patterns if arrays are null or empty
            if (iOSPatterns == null || iOSPatterns.Length == 0)
            {
                InitializeDefaultPatterns();
            }
        }

        private void InitializeDefaultPatterns()
        {
            iOSPatterns = new VibrationPattern[]
            {
                new VibrationPattern(VibrationType.Selection, new float[] { 0.0f, 0.04f }, new float[] { 0.471f, 0.471f }),
                new VibrationPattern(VibrationType.LightImpact, new float[] { 0.000f, 0.040f }, new float[] { 0.156f, 0.156f }),
                new VibrationPattern(VibrationType.MediumImpact, new float[] { 0.000f, 0.080f }, new float[] { 0.471f, 0.471f }),
                new VibrationPattern(VibrationType.HeavyImpact, new float[] { 0.0f, 0.16f }, new float[] { 1.0f, 1.00f }),
                new VibrationPattern(VibrationType.RigidImpact, new float[] { 0.0f, 0.04f }, new float[] { 1.0f, 1.00f }),
                new VibrationPattern(VibrationType.SoftImpact, new float[] { 0.000f, 0.160f }, new float[] { 0.156f, 0.156f }),
                new VibrationPattern(VibrationType.Success, new float[] { 0.0f, 0.040f, 0.080f, 0.240f }, new float[] { 0.0f, 0.157f, 0.000f, 1.000f }),
                new VibrationPattern(VibrationType.Failure, new float[] { 0.0f, 0.080f, 0.120f, 0.200f, 0.240f, 0.400f, 0.440f, 0.480f }, new float[] { 0.0f, 0.470f, 0.000f, 0.470f, 0.000f, 1.000f, 0.000f, 0.157f }),
                new VibrationPattern(VibrationType.Warning, new float[] { 0.0f, 0.120f, 0.240f, 0.280f }, new float[] { 0.0f, 1.000f, 0.000f, 0.470f })
            };

            androidPatterns = new VibrationPattern[]
            {
                new VibrationPattern(VibrationType.Selection, new float[] { 0.0f, 0.04f }, new float[] { 0.471f, 0.471f }),
                new VibrationPattern(VibrationType.LightImpact, new float[] { 0.000f, 0.040f }, new float[] { 0.156f, 0.156f }),
                new VibrationPattern(VibrationType.MediumImpact, new float[] { 0.000f, 0.080f }, new float[] { 0.471f, 0.471f }),
                new VibrationPattern(VibrationType.HeavyImpact, new float[] { 0.0f, 0.16f }, new float[] { 1.0f, 1.00f }),
                new VibrationPattern(VibrationType.RigidImpact, new float[] { 0.0f, 0.04f }, new float[] { 1.0f, 1.00f }),
                new VibrationPattern(VibrationType.SoftImpact, new float[] { 0.000f, 0.160f }, new float[] { 0.156f, 0.156f }),
                new VibrationPattern(VibrationType.Success, new float[] { 0.0f, 0.040f, 0.080f, 0.240f }, new float[] { 0.0f, 0.157f, 0.000f, 1.000f }),
                new VibrationPattern(VibrationType.Failure, new float[] { 0.0f, 0.080f, 0.120f, 0.200f, 0.240f, 0.400f, 0.440f, 0.480f }, new float[] { 0.0f, 0.470f, 0.000f, 0.470f, 0.000f, 1.000f, 0.000f, 0.157f }),
                new VibrationPattern(VibrationType.Warning, new float[] { 0.0f, 0.120f, 0.240f, 0.280f }, new float[] { 0.0f, 1.000f, 0.000f, 0.470f })
            };
        }

        /// <summary>
        /// Get the appropriate pattern based on platform and pattern type
        /// </summary>
        public VibrationPattern GetPattern(VibrationType type)
        {
#if UNITY_IOS && !UNITY_EDITOR
            return GetPatternFromArray(iOSPatterns, type);
#elif UNITY_ANDROID && !UNITY_EDITOR
            return GetPatternFromArray(androidPatterns, type);
#else
            // Default to iOS patterns in editor
            return GetPatternFromArray(iOSPatterns, type);
#endif
        }

        private VibrationPattern GetPatternFromArray(VibrationPattern[] patterns, VibrationType type)
        {
            foreach (var pattern in patterns)
            {
                if (pattern.VibrationType == type)
                {
                    return pattern;
                }
            }
            // Return medium impact as default
            foreach (var pattern in patterns)
            {
                if (pattern.VibrationType == VibrationType.MediumImpact)
                {
                    return pattern;
                }
            }
            return null;
        }
    }
}
