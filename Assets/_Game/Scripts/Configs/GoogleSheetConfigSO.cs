using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class GoogleSheetConfigSO<T> : GameConfig where T : new()
    {

#if UNITY_EDITOR
        [SerializeField] protected string _sheetId;
        [SerializeField] protected string _gridId;
        [SerializeField] protected string _tsvCachePath;
        [SerializeField] protected string _jsonCachePath;
        [SerializeField] protected List<T> _datas = new();

        [Alchemy.Inspector.Button]
        public void OpenSheet() =>
            Application.OpenURL($"https://docs.google.com/spreadsheets/d/{_sheetId}/edit#gid={_gridId}");

        [Alchemy.Inspector.Button]
        protected void Sync() =>
            GoogleSheetHelper.GetConfig<T>(_sheetId, _gridId, OnSynced, _tsvCachePath, _jsonCachePath);

        public void SyncRuntime(Action callback = null)
        {
            GoogleSheetHelper.GetConfig<T>(_sheetId, _gridId, data =>
            {
                OnSynced(data);
                callback?.Invoke();
            });
        }

        protected virtual void OnSynced(List<T> googleSheetData)
        {
            _datas = googleSheetData;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}