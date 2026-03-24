using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbarExtender;

[InitializeOnLoad]
public class BetterPlayButtonEditor
{
    #region Fields

    public const string k_PreferenceSection = "Editor";
    private const string _lockStartupSceneKey = "LockStartupScene";
    private const string _timeScaleLabel = "Time Scale";
    [SerializeField] private static float _minTimeScale = 0.1f;
    [SerializeField] private static float _maxTimeScale = 3f;
    [SerializeField] private static float _defaultTimeScale = 1f;
    private static float _timeScale = _defaultTimeScale;

    #endregion

    #region Unity Event Functions

    static BetterPlayButtonEditor()
    {
        ToolbarExtender.LeftToolbarGUI.Add(OnToolbarLeftGUI);
        ToolbarExtender.RightToolbarGUI.Add(OnToolbarRightGUI);
        EditorApplication.delayCall += SetStartupScene;
    }

    #endregion

    #region Methods

    static void OnToolbarLeftGUI()
    {
        GUILayout.FlexibleSpace();

        var lockStartup = LockStartupScene;
        if (EditorBuildSettings.scenes.Length > 0)
        {
            string sceneName = Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[0].path);
            lockStartup = GUILayout.Toggle(lockStartup,
                new GUIContent($"Start {sceneName} scene", null, $"Always start play mode with {sceneName} scene"));
            LockStartupScene = lockStartup;
            if (lockStartup)
            {
                SetStartupScene();
            }
            else
            {
                UnsetStartupScene();
            }
        }

        GUILayout.Space(20);
        GUILayout.Label("Open Scene:");
        List<AddressableAssetEntry> listAsset = GetAllSceneEntrys();
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        List<string> listScenePath = new List<string>();
        List<string> listNameScene = new List<string>();
        string currentScenePath = SceneManager.GetActiveScene().path;

        for (int i = 0; i < scenes.Length; i++)
        {
            string pathScene = scenes[i].path;
            listScenePath.Add(pathScene);
            string nameScene = Path.GetFileNameWithoutExtension(pathScene);
            listNameScene.Add(nameScene);
        }
        for (int i = 0; i < listAsset.Count; i++)
        {
            string pathAsset = listAsset[i].AssetPath;
            if (!listScenePath.Contains(pathAsset))
            {
                listScenePath.Add(pathAsset);
                string nameScene = Path.GetFileNameWithoutExtension(pathAsset);
                listNameScene.Add(nameScene);
            }
        }

        int currentIndex = listScenePath.FindIndex(path => path == currentScenePath);
        currentIndex = currentIndex < 0 ? 0 : currentIndex;
        listNameScene.Add("<-unknown->");
        var sceneIndex = EditorGUILayout.Popup(
            currentIndex, listNameScene.ToArray(), GUILayout.Width(150));
        if (sceneIndex != currentIndex && sceneIndex < listScenePath.Count)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(listScenePath[sceneIndex]);
            }
        }
        GUILayout.Space(20);
    }

    static void OnToolbarRightGUI()
    {
        // Time scale controls in a horizontal layout
        GUILayout.BeginHorizontal(GUILayout.Width(250));
        GUILayout.Label(_timeScaleLabel, GUILayout.Width(70));
        float newTimeScale = GUILayout.HorizontalSlider(_timeScale, _minTimeScale, _maxTimeScale, GUILayout.Width(80));
        newTimeScale = Mathf.Round(newTimeScale * 100f) / 100f;
        GUILayout.Label(newTimeScale.ToString("0.00"), GUILayout.Width(40));
        if (GUILayout.Button("Reset", GUILayout.Width(50)))
        {
            newTimeScale = _defaultTimeScale;
        }
        GUILayout.EndHorizontal();

        if (!Mathf.Approximately(newTimeScale, _timeScale))
        {
            _timeScale = newTimeScale;
            if (EditorApplication.isPlaying)
            {
                Time.timeScale = _timeScale;
            }
        }
        if (!EditorApplication.isPlaying && !Mathf.Approximately(Time.timeScale, 1f))
        {
            Time.timeScale = 1f;
        }

        GUILayout.FlexibleSpace();
    }

    private static bool LockStartupScene
    {
        get
        {
            if (!PlayerPrefs.HasKey(_lockStartupSceneKey))
            {
                PlayerPrefs.SetInt(_lockStartupSceneKey, 1);
            }
            return PlayerPrefs.GetInt(_lockStartupSceneKey, 1) > 0;
        }
        set => PlayerPrefs.SetInt(_lockStartupSceneKey, value ? 1 : 0);
    }

    static void SetStartupScene()
    {
        if (LockStartupScene)
        {
            if (EditorBuildSettings.scenes.Length > 0)
            {
                var scenePath = EditorBuildSettings.scenes[0].path;

                if (!string.IsNullOrEmpty(scenePath))
                {
                    SetPlayModeStartScene(scenePath);
                }
            }
        }
    }

    static void UnsetStartupScene()
    {
        EditorSceneManager.playModeStartScene = null;
    }

    static void SetPlayModeStartScene(string scenePath)
    {
        SceneAsset myWantedStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
        if (myWantedStartScene != null)
            EditorSceneManager.playModeStartScene = myWantedStartScene;
        else
            Debug.LogError("Could not find Scene " + scenePath);
    }

    public static List<AddressableAssetEntry> GetAllSceneEntrys()
    {
        var setting = AddressableAssetSettingsDefaultObject.GetSettings(false);
        var entrys = new List<AddressableAssetEntry>();

        if (setting != default)
        {
            foreach (var group in setting.groups)
            {
                var _entrys = new List<AddressableAssetEntry>();
                group.GatherAllAssets(_entrys, true, true, true, e => e.IsScene);
                entrys.AddRange(_entrys);
            }
        }
        return entrys;
    }

    #endregion
}