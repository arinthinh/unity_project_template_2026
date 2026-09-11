using UnityEngine;

public abstract class DataModule : ScriptableObject
{
    public string DataKey => GetType().FullName;

    public abstract void Init();

    public abstract void NewData();

    public virtual void SaveData() => GameDataManager.Instance.SaveData(DataKey, this);

    public virtual void LoadData() => GameDataManager.Instance.LoadDataOverwrite(DataKey, this);
}
