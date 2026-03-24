using UnityEngine;
using System;
using System.Globalization;

#if (UNITY_ANDROID && !UNITY_EDITOR)
using System.Text;
#elif (UNITY_IOS && !UNITY_EDITOR)
using UnityEngine.iOS;
#endif

namespace Amanotes.Orchestra
{
    /// <summary>
    /// Master static class for vibration control on iOS and Android
    /// Automatically initializes on first use
    /// </summary>
    public static class KVibrations
    {
        private static KVibrationsSettings _settings;
        private static bool _isInitialized = false;
        private static float _outputLevel = 1.0f;
        private static readonly NumberFormatInfo numberFormat = new() { NumberDecimalSeparator = "." };

        private static String emphasisTemplate;
        private static String constantTemplate;

        private const string SETTINGS_RESOURCE_PATH = "KVibrationsSettings";
        private const string PREFS_KEY_VIBRATIONS_ENABLED = "KVibrations_VibrationsEnabled";

        public static bool VibrationsEnabled
        {
            get => PlayerPrefs.GetInt(PREFS_KEY_VIBRATIONS_ENABLED, 1) == 1;
            set
            {
                PlayerPrefs.SetInt(PREFS_KEY_VIBRATIONS_ENABLED, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// The overall vibration output level (0.0 to 1.0)
        /// </summary>
        public static float outputLevel
        {
            get { return _outputLevel; }
            set { _outputLevel = Mathf.Clamp01(value); }
        }

        /// <summary>
        /// Auto-initializes the controller if not already initialized
        /// Loads settings from Resources folder or creates default settings
        /// </summary>
        private static void EnsureInitialized()
        {
            if (_isInitialized)
                return;

            // Try to load settings from Resources
            _settings = Resources.Load<KVibrationsSettings>(SETTINGS_RESOURCE_PATH);

            // If no settings found, create default settings
            if (_settings == null)
            {
                Debug.LogWarning($"KVibrationsSettings not found in Resources. Creating default settings. " +
                                 $"To customize, create a KVibrationsSettings asset in a Resources folder.");
                _settings = ScriptableObject.CreateInstance<KVibrationsSettings>();
            }

            // Load templates for custom patterns
            emphasisTemplate = (Resources.Load("nv-emphasis-template") as TextAsset)?.text;
            constantTemplate = (Resources.Load("nv-constant-template") as TextAsset)?.text;

            // Initialize platform-specific haptics
            if (DeviceCapabilities.isVersionSupported)
            {
                LofeltHaptics.Initialize();
                DeviceCapabilities.Init();
            }

            _isInitialized = true;
        }

        /// <summary>
        /// Play a vibration using a predefined pattern type
        /// </summary>
        public static void PlayPattern(VibrationType vibrationType)
        {
            EnsureInitialized();
            
            if (!VibrationsEnabled || vibrationType == VibrationType.None)
                return;

            if (_settings == null)
            {
                Debug.LogError("KVibrations failed to initialize settings.");
                return;
            }

            VibrationPattern pattern = _settings.GetPattern(vibrationType);
            if (pattern == null)
            {
                Debug.LogWarning($"Pattern {vibrationType} not found in settings.");
                return;
            }

            PlayCustomPattern(pattern.amplitude, pattern.time);
        }

        /// <summary>
        /// Play a custom vibration pattern with amplitude and time arrays
        /// </summary>
        public static void PlayCustomPattern(float[] amplitude, float[] time)
        {
            EnsureInitialized();
            
            if (!VibrationsEnabled)
                return;

            if (amplitude == null || time == null || amplitude.Length != time.Length || amplitude.Length == 0)
            {
                Debug.LogWarning("Invalid custom pattern. Amplitude and time arrays must be non-null, same length, and non-empty.");
                return;
            }

            if (!DeviceCapabilities.isVersionSupported)
                return;

#if (UNITY_IOS && !UNITY_EDITOR)
            // On iOS, use native preset haptics
            VibrationType presetType = GetClosestPresetForAmplitude(amplitude[0]);
            if (presetType != VibrationType.None)
            {
                LofeltHaptics.TriggerPresetHaptics((int)presetType);
            }
#elif (UNITY_ANDROID && !UNITY_EDITOR)
            // On Android, convert pattern to JSON and play
            String json = PatternToClip(time, amplitude);
            if (!string.IsNullOrEmpty(json))
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(json);
                LofeltHaptics.Load(data);
                LofeltHaptics.SetAmplitudeMultiplication(_outputLevel);
                LofeltHaptics.Play();
            }
            else
            {
                // Fallback to maximum amplitude pattern
                LofeltHaptics.PlayMaximumAmplitudePattern(time);
            }
#endif
        }

        /// <summary>
        /// Play a single emphasis point (quick vibration)
        /// </summary>
        public static void PlayEmphasis(float amplitude, float frequency)
        {
            EnsureInitialized();
            
            if (!VibrationsEnabled)
                return;

            if (!DeviceCapabilities.isVersionSupported)
                return;

            float clampedAmplitude = Mathf.Clamp(amplitude, 0.0f, 1.0f);
            float clampedFrequency = Mathf.Clamp(frequency, 0.0f, 1.0f);
            const float duration = 0.1f;

#if (UNITY_IOS && !UNITY_EDITOR)
            VibrationType preset = GetClosestPresetForAmplitude(clampedAmplitude);
            LofeltHaptics.TriggerPresetHaptics((int)preset);
#elif (UNITY_ANDROID && !UNITY_EDITOR)
            if (emphasisTemplate == null)
            {
                Debug.LogWarning("Emphasis template not found.");
                return;
            }

            String json = emphasisTemplate
                .Replace("{amplitude}", clampedAmplitude.ToString(numberFormat))
                .Replace("{frequency}", clampedFrequency.ToString(numberFormat))
                .Replace("{duration}", duration.ToString(numberFormat));

            byte[] data = System.Text.Encoding.UTF8.GetBytes(json);
            LofeltHaptics.Load(data);
            LofeltHaptics.SetAmplitudeMultiplication(_outputLevel);
            LofeltHaptics.Play();
#endif
        }

        /// <summary>
        /// Play a constant vibration for a specified duration
        /// </summary>
        public static void PlayConstant(float amplitude, float frequency, float duration)
        {
            EnsureInitialized();
            
            if (!VibrationsEnabled)
                return;

            if (!DeviceCapabilities.isVersionSupported)
                return;

            float clampedAmplitude = Mathf.Clamp(amplitude, 0.0f, 1.0f);
            float clampedFrequency = Mathf.Clamp(frequency, 0.0f, 1.0f);
            float clampedDuration = Mathf.Max(duration, 0.0f);

#if (UNITY_IOS && !UNITY_EDITOR)
            LofeltHaptics.TriggerPresetHaptics((int)VibrationType.HeavyImpact);
#elif (UNITY_ANDROID && !UNITY_EDITOR)
            if (constantTemplate == null)
            {
                Debug.LogWarning("Constant template not found.");
                return;
            }

            String json = constantTemplate.Replace("{duration}", clampedDuration.ToString(numberFormat));
            byte[] data = System.Text.Encoding.UTF8.GetBytes(json);
            LofeltHaptics.Load(data);
            LofeltHaptics.SetAmplitudeMultiplication(_outputLevel * clampedAmplitude);
            LofeltHaptics.SetFrequencyShift(clampedFrequency);
            LofeltHaptics.Play();
#endif
        }

        /// <summary>
        /// Stop any currently playing vibration
        /// </summary>
        public static void Stop()
        {
            if (DeviceCapabilities.isVersionSupported)
            {
                LofeltHaptics.Stop();
                LofeltHaptics.StopPattern();
            }
        }

        private static VibrationType GetClosestPresetForAmplitude(float amplitude)
        {
            if (amplitude > 0.5f)
                return VibrationType.HeavyImpact;
            else if (amplitude > 0.3f)
                return VibrationType.MediumImpact;
            else
                return VibrationType.LightImpact;
        }

        private static String PatternToClip(float[] time, float[] amplitude)
        {
            TextAsset clipJsonTemplate = Resources.Load("nv-pattern-template") as TextAsset;
            if (clipJsonTemplate == null)
                return "";

            String amplitudeEnvelope = "";
            for (int i = 0; i < time.Length; i++)
            {
                float clampedAmplitude = Mathf.Clamp(amplitude[i], 0.0f, 1.0f);
                amplitudeEnvelope += "{ \"time\":" + time[i].ToString(numberFormat) + "," +
                                     "\"amplitude\":" + clampedAmplitude.ToString(numberFormat) + "}";

                if (i + 1 < time.Length)
                {
                    amplitudeEnvelope += ",";
                }
            }

            return clipJsonTemplate.text.Replace("{amplitude-envelope}", amplitudeEnvelope);
        }
    }
}