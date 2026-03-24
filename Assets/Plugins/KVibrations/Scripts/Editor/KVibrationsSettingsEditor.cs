#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Amanotes.Orchestra
{
    [CustomEditor(typeof(KVibrationsSettings))]
    public class KVibrationsSettingsEditor : Editor
    {
        private bool iOSFoldout = true;
        private bool androidFoldout = true;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            KVibrationsSettings settings = (KVibrationsSettings)target;

            // Title
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUILayout.LabelField("Vibration Patterns Configuration", titleStyle);
            EditorGUILayout.Space(5);

            // Reset Button
            if (GUILayout.Button(new GUIContent("Reset to Default Values", "Reset all patterns to their default values")))
            {
                if (EditorUtility.DisplayDialog("Reset Patterns",
                    "Are you sure you want to reset all patterns to default values?",
                    "Yes", "Cancel"))
                {
                    ResetToDefaults(settings);
                }
            }

            EditorGUILayout.Space(10);

            // iOS Patterns Foldout
            iOSFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(iOSFoldout, new GUIContent("iOS Patterns", "Vibration patterns for iOS devices"));
            if (iOSFoldout)
            {
                EditorGUI.indentLevel++;
                DrawPatternList(settings.iOSPatterns);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space(10);

            // Android Patterns Foldout
            androidFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(androidFoldout, new GUIContent("Android Patterns", "Vibration patterns for Android devices"));
            if (androidFoldout)
            {
                EditorGUI.indentLevel++;
                DrawPatternList(settings.androidPatterns);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPatternList(VibrationPattern[] patterns)
        {
            if (patterns == null || patterns.Length == 0)
            {
                EditorGUILayout.HelpBox("No patterns available", MessageType.Info);
                return;
            }

            foreach (var pattern in patterns)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                // Title with vibration type
                EditorGUILayout.LabelField(pattern.VibrationType.ToString(), EditorStyles.boldLabel);

                // Editable Duration
                float duration = pattern.GetDuration();
                float newDuration = EditorGUILayout.FloatField(
                    new GUIContent("Duration (s)", "Duration of the vibration pattern in seconds"),
                    duration);

                if (newDuration != duration && newDuration > 0)
                {
                    UpdateDuration(pattern, newDuration);
                }

                // Editable Amplitude
                if (pattern.amplitude != null && pattern.amplitude.Length > 0)
                {
                    float amplitude = pattern.amplitude[0];
                    float newAmplitude = EditorGUILayout.Slider(
                        new GUIContent("Amplitude", "Intensity of the vibration (0.0 to 1.0)"),
                        amplitude, 0f, 1f);

                    if (newAmplitude != amplitude)
                    {
                        UpdateAmplitude(pattern, newAmplitude);
                    }
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }
        }

        private void UpdateDuration(VibrationPattern pattern, float newDuration)
        {
            if (pattern.time == null || pattern.time.Length == 0) return;

            float oldDuration = pattern.GetDuration();
            if (oldDuration <= 0) return;

            float scale = newDuration / oldDuration;
            for (int i = 0; i < pattern.time.Length; i++)
            {
                pattern.time[i] *= scale;
            }
        }

        private void UpdateAmplitude(VibrationPattern pattern, float newAmplitude)
        {
            if (pattern.amplitude == null) return;

            for (int i = 0; i < pattern.amplitude.Length; i++)
            {
                pattern.amplitude[i] = newAmplitude;
            }
        }

        private void ResetToDefaults(KVibrationsSettings settings)
        {
            settings.iOSPatterns = new VibrationPattern[]
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

            settings.androidPatterns = new VibrationPattern[]
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

            EditorUtility.SetDirty(settings);
        }
    }
}
#endif