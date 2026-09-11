using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LoadingState _loadingState;

    private async UniTask Start()
    {
        await UniTask.Yield();
        ConfigSettings();
        _loadingState.Enter();
    }

    private void ConfigSettings()
    {
        Input.multiTouchEnabled = false;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
    
}
 
