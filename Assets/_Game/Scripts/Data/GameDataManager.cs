using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Alchemy.Inspector;
using UnityEngine;

public class GameDataManager : MonoSingleton<GameDataManager>
{
    private const string FILE_EXTENSION = ".json";

    public List<DataModule> DataList = new();

    private string SavePath => Path.Combine(Application.persistentDataPath, "Saves");

    protected void Awake()
    {
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
    }


    private void CreateFolder()
    {
        if (Directory.Exists(SavePath)) return;

        Directory.CreateDirectory(SavePath);
        Debug.Log("save path : " + SavePath);
    }

    public T GetData<T>() where T : DataModule
    {
        try
        {
            return DataList.Find(x => x.GetType().FullName == typeof(T).FullName) as T;
        }
        catch (Exception)
        {
            Debug.LogErrorFormat("Missing DataModule: {0}", typeof(T).FullName);
            return null;
        }
    }


    #region SAVE

    public void SaveData<T>(string key, T userSaveData)
    {
        byte[] serializedBytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(userSaveData));
        var savePath = Path.Combine(SavePath, key + FILE_EXTENSION);
        SimpleEncrypt(ref serializedBytes);
        File.WriteAllBytes(savePath, serializedBytes);
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

    public void LoadDataOverwrite(string key, object target)
    {
        var savePath = Path.Combine(SavePath, key + FILE_EXTENSION);
        if (!File.Exists(savePath)) return;
        byte[] encryptedBytes = File.ReadAllBytes(savePath);
        SimpleEncrypt(ref encryptedBytes);
        JsonUtility.FromJsonOverwrite(Encoding.UTF8.GetString(encryptedBytes), target);
    }

    #endregion

    public bool HasData(string key)
    {
        return File.Exists(Path.Combine(SavePath, key + FILE_EXTENSION));
    }

    public void LoadAllData()
    {
        foreach (var data in DataList)
        {
            data.LoadData();
        }
    }

    public void SaveAllData()
    {
        foreach (var data in DataList)
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
}
