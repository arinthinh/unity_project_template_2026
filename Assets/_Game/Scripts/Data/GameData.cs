using UnityEngine;

public abstract class GameData : MonoBehaviour
{
    public string DataKey => GetType().FullName;

    public abstract void Init();

    public abstract void NewData();

    public abstract void LoadData();

    public abstract void SaveData();
}