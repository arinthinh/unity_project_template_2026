using UnityEngine;

public class MainCamera : MonoSingleton<MainCamera>
{
    private Camera _mainCamera;
    public Camera Camera => _mainCamera;
    public Transform CameraTransform => transform;
    
    protected void Awake()
    {
        _mainCamera = GetComponent<Camera>();
    }

}       