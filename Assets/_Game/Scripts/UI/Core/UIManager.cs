using System;
using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;

public enum EUILayer
{
    Screen,
    Popup,
    AlwaysOnTop
}

/// <summary>

public sealed class UIManager : SingletonMonoBehaviour<UIManager>
{
    [Header("CAMERA")]
    [SerializeField] private Camera _uiCamera;

    [Header("UI CONTAINERS")]
    [SerializeField] private Transform _screenContainer;
    [SerializeField] private Transform _popupContainer;

    [Header("UI PREFABS")]
    [SerializeField] private ScreenBase[] _screenPrefabs = Array.Empty<ScreenBase>();
    [SerializeField] private PopupBase[] _popupPrefabs = Array.Empty<PopupBase>();

    private readonly Dictionary<string, ScreenBase> _loadedScreens = new();
    private readonly Dictionary<string, PopupBase> _loadedPopups = new();

    public Camera UICamera => _uiCamera;

    #region Methods

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;
    }

    private ScreenBase GetOrLoadScreen(Type type)
    {
        var screenName = type.FullName ?? string.Empty;
        if (_loadedScreens.TryGetValue(screenName, out var screen))
            return screen;

        for (int i = 0; i < _screenPrefabs.Length; i++)
        {
            var prefab = _screenPrefabs[i];
            if (prefab.GetType() == type)
            {
                var instance = Instantiate(prefab, _screenContainer);
                _loadedScreens.Add(screenName, instance);
                instance.OnInit(this);
                ReorderLoadedScreens();
                return instance;
            }
        }
        return null;
    }

    private PopupBase GetOrLoadPopup(Type type)
    {
        var popupName = type.FullName ?? string.Empty;
        if (_loadedPopups.TryGetValue(popupName, out var popup))
            return popup;

        for (int i = 0; i < _popupPrefabs.Length; i++)
        {
            var prefab = _popupPrefabs[i];
            if (prefab.GetType() == type)
            {
                var instance = Instantiate(prefab, _popupContainer);
                _loadedPopups.Add(popupName, instance);
                instance.OnInit(this);
                ReorderLoadedPopups();
                return instance;
            }
        }
        return null;
    }

    public T ShowScreen<T>() where T : ScreenBase
    {
        var screen = GetOrLoadScreen(typeof(T)) as T;
        if (screen == null)
        {
            Debug.LogError("Invalid screen " + typeof(T).FullName);
            return null;
        }
        screen.Show();
        return screen;
    }

    public T HideScreen<T>() where T : ScreenBase
    {
        var screen = GetOrLoadScreen(typeof(T)) as T;
        if (screen == null)
        {
            Debug.LogError("Invalid screen " + typeof(T).FullName);
            return null;
        }
        screen.Hide();
        return screen;
    }

    public T GetScreen<T>() where T : ScreenBase
    {
        return GetOrLoadScreen(typeof(T)) as T;
    }

    public void ReleaseScreen<T>() where T : ScreenBase
    {
        var screenName = typeof(T).FullName ?? string.Empty;
        if (_loadedScreens.TryGetValue(screenName, out var screen))
        {
            Destroy(screen.gameObject);
            _loadedScreens.Remove(screenName);
            ReorderLoadedScreens();
        }
    }

    public T ShowPopup<T>() where T : PopupBase
    {
        var popup = GetOrLoadPopup(typeof(T)) as T;
        if (popup == null)
        {
            Debug.LogError("Invalid popup " + typeof(T).FullName);
            return null;
        }
        popup.Show();
        return popup;
    }

    public T HidePopup<T>() where T : PopupBase
    {
        var popup = GetOrLoadPopup(typeof(T)) as T;
        if (popup == null)
        {
            Debug.LogError("Invalid popup " + typeof(T).FullName);
            return null;
        }
        popup.Hide();
        return popup;
    }

    public T GetPopup<T>() where T : PopupBase
    {
        return GetOrLoadPopup(typeof(T)) as T;
    }

    public void ReleasePopup<T>() where T : PopupBase
    {
        var popupName = typeof(T).FullName ?? string.Empty;
        if (_loadedPopups.TryGetValue(popupName, out var popup))
        {
            Destroy(popup.gameObject);
            _loadedPopups.Remove(popupName);
            ReorderLoadedPopups();
        }
    }

    // Ensures loaded screens are always in the correct order in the container
    private void ReorderLoadedScreens()
    {
        for (int i = 0; i < _screenPrefabs.Length; i++)
        {
            var prefab = _screenPrefabs[i];
            var screenName = prefab.GetType().FullName ?? string.Empty;
            if (_loadedScreens.TryGetValue(screenName, out var loadedScreen) && loadedScreen != null)
            {
                loadedScreen.transform.SetSiblingIndex(i);
            }
        }
    }

    // Ensures loaded popups are always in the correct order in the container
    private void ReorderLoadedPopups()
    {
        for (int i = 0; i < _popupPrefabs.Length; i++)
        {
            var prefab = _popupPrefabs[i];
            var popupName = prefab.GetType().FullName ?? string.Empty;
            if (_loadedPopups.TryGetValue(popupName, out var loadedPopup) && loadedPopup != null)
            {
                loadedPopup.transform.SetSiblingIndex(i);
            }
        }
    }

    #endregion
}