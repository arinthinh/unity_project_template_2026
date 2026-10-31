using System;
using System.IO;
using UnityEngine;

public static class AData
{
    private const string FILE_NAME = "save.json";

    public static SaveData Data { get; private set; }

    private static string SaveFolder => Path.Combine(Application.persistentDataPath, "Saves");
    private static string SavePath => Path.Combine(SaveFolder, FILE_NAME);
    private static string TempPath => SavePath + ".tmp";
    private static string BackupPath => SavePath + ".bak";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        Directory.CreateDirectory(SaveFolder);
        Load();
        // Unsubscribe first so the handler isn't added twice when Domain Reload is disabled.
        Application.quitting -= Save;
        Application.quitting += Save;
    }

    // Write to a temp file first, then swap, so a crash mid-write never corrupts the existing save.
    public static void Save()
    {
        File.WriteAllText(TempPath, JsonUtility.ToJson(Data, true));
        if (File.Exists(SavePath))
            File.Replace(TempPath, SavePath, BackupPath);
        else
            File.Move(TempPath, SavePath);
    }

    public static void Load() => Data = TryRead(SavePath) ?? TryRead(BackupPath) ?? new SaveData();

    public static void ResetData()
    {
        Data = new();
        Save();
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Delete Save")]
#endif
    public static void DeleteSave()
    {
        File.Delete(SavePath);
        File.Delete(BackupPath);
        File.Delete(TempPath);
        Data = new();
        Debug.Log("Save deleted: " + SaveFolder);
    }

    private static SaveData TryRead(string path)
    {
        if (!File.Exists(path)) return null;

        try
        {
            var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
            if (data == null) Debug.LogError("Save file is empty: " + path);
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to read save file {path}: {e.Message}");
            return null;
        }
    }
}
