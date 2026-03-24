using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Alchemy.Inspector;
using UnityEngine;

public enum ESaveType
{
    PlayerPrefs,
    File
}

public class DataManager : SingletonMonoBehaviour<DataManager>
{
    public ESaveType SaveType;
    public List<GameData> DataList = new();

    private string SavePath => Path.Combine(Application.persistentDataPath, "Saves");

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;
        CreateFolder();
        foreach (var data in DataList)
        {
            data.Init();
            if (!HasData(data.DataKey))
            {
                data.NewData();
            }
            else
            {
                data.LoadData();
            }
            data.SaveData();
        }
    }

    [ContextMenu("DeleteSave")]
    [Button]
    public void DeleteSave()
    {
        switch (SaveType)
        {
            case ESaveType.PlayerPrefs:
                PlayerPrefs.DeleteAll();
                break;

            case ESaveType.File:
                try
                {
                    if (Directory.Exists(SavePath))
                    {
                        Debug.Log("Folder exists. Deleting the folder...");

                        // Delete the folder and its contents
                        Directory.Delete(SavePath, true);

                        Debug.Log("Folder deleted.");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
                break;
        }
    }


    private void CreateFolder()
    {
        if (SaveType == ESaveType.PlayerPrefs)
            return;

        if (Directory.Exists(SavePath)) return;

        Directory.CreateDirectory(SavePath);
        Debug.Log("save path : " + SavePath);
    }

    public T GetData<T>() where T : GameData
    {
        try
        {
            return DataList.Find(x => x.GetType().FullName == typeof(T).FullName) as T;
        }
        catch (Exception)
        {
            Debug.LogErrorFormat("Missing GameData: {0}", typeof(T).FullName);
            return null;
        }
    }


    #region SAVE

    public void SaveData<T>(string key, T userSaveData)
    {
        byte[] serializedBytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(userSaveData));
        switch (SaveType)
        {
            case ESaveType.PlayerPrefs:
                // Convert bytes to Base64 string for PlayerPrefs
                var encodedText = Convert.ToBase64String(serializedBytes);
                SaveDataToPlayerPrefs(key, encodedText);
                break;
            case ESaveType.File:
                SaveDataToFile(key, serializedBytes);
                break;
            default:
                var defaultEncodedText = Convert.ToBase64String(serializedBytes);
                SaveDataToPlayerPrefs(key, defaultEncodedText);
                break;
        }
    }

    private void SaveDataToPlayerPrefs(string key, string data)
    {
        PlayerPrefs.SetString(key, data);
        PlayerPrefs.Save();
    }

    private void SaveDataToFile(string key, byte[] data)
    {
        var savePath = Path.Combine(SavePath, key);
        SimpleEncrypt(ref data);
        File.WriteAllBytes(savePath, data);
    }

    private void SimpleEncrypt(ref byte[] data)
    {
        if (Application.isEditor)
            return;

        byte[] key = Encoding.UTF8.GetBytes(SystemInfo.deviceUniqueIdentifier);
        int kLen = key.Length;
        for (uint i = 0; i < data.Length; i++)
            data[i] ^= key[i % kLen];
    }

    #endregion

    #region LOAD

    public T LoadData<T>(string key)
    {
        switch (SaveType)
        {
            case ESaveType.PlayerPrefs:
                return LoadDataFromPlayerPrefs<T>(key);
            case ESaveType.File:
                return LoadDataFromFile<T>(key);
            default:
                return LoadDataFromPlayerPrefs<T>(key);
        }
    }


// For PlayerPrefs
    private T LoadDataFromPlayerPrefs<T>(string key)
    {
        string encodedText = PlayerPrefs.GetString(key);
        if (string.IsNullOrEmpty(encodedText)) return default;
        byte[] encryptedBytes = Convert.FromBase64String(encodedText);
        T data = JsonUtility.FromJson<T>(Encoding.UTF8.GetString(encryptedBytes));
        return data;
    }

// For File
    private T LoadDataFromFile<T>(string key)
    {
        var savePath = Path.Combine(SavePath, key);
        if (!File.Exists(savePath)) return default;
        byte[] encryptedBytes = File.ReadAllBytes(savePath);
        T data = JsonUtility.FromJson<T>(Encoding.UTF8.GetString(encryptedBytes));
        return data;
    }

    #endregion

    public bool HasData(string key)
    {
        return SaveType switch
        {
            ESaveType.PlayerPrefs => PlayerPrefs.HasKey(key),
            ESaveType.File => File.Exists(Path.Combine(SavePath, key)),
            _ => PlayerPrefs.HasKey(key)
        };
    }

    public void LoadAllData()
    {
        foreach (GameData data in DataList)
        {
            data.LoadData();
        }
    }

    public void SaveAllData()
    {
        foreach (GameData data in DataList)
        {
            data.SaveData();
        }
    }

    public void DeleteAllData()
    {
        foreach (var data in DataList)
        {
            data.NewData();
        }
    }

    private void OnApplicationQuit()
    {
        SaveAllData();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveAllData();
        }
    }

    #region HELPERS

#if UNITY_EDITOR
    private void OnValidate()
    {
        DataList.Clear();
        GetComponents(DataList);
    }
#endif

    #endregion
}