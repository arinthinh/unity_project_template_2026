using Cysharp.Threading.Tasks;
using Framework;
using UnityEngine;

public class LoadingPipeline : MonoBehaviour
{
    [SerializeField] private float _loadingDuration = 2f;
    [SerializeField] private int _targetFrameRate = 60;

    // Services
    private DataManager _dataManager;
    private AudioManager _audioManager;
    private UIManager _uiManager;

    // Init game scene
    private async UniTaskVoid Start()
    {
        await UniTask.Yield();

        // Get services
        _dataManager = DataManager.Instance;
        _audioManager = AudioManager.Instance;
        _uiManager = UIManager.Instance;

        // Init game.
        Input.multiTouchEnabled = false;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = _targetFrameRate;
    }
}