using System;
using MemoryPack;

[Serializable]
public class UserSaveData
{
}

public class UserData : GameData
{
    public UserSaveData Data;

    public override void Init()
    {
    }
    
    public override void SaveData() => DataManager.Instance.SaveData(DataKey, Data);

    public override void NewData()
    {
        Data = new UserSaveData
        {
        };
    }

    public override void LoadData()
    {
        Data = DataManager.Instance.LoadData<UserSaveData>(DataKey);
    }
}