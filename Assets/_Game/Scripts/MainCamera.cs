using UnityEngine;

public class MainCamera : SingletonMonoBehaviour<MainCamera>
{
    private Camera _mainCamera;
    public Camera Camera => _mainCamera;
    public Transform CameraTransform => transform;
    
    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;
        _mainCamera = GetComponent<Camera>();
    }

}       