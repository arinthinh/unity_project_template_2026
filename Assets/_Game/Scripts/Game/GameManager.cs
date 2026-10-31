using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("STATES")]
    [SerializeField] private LoadingState _loadingState;
    [SerializeField] private TitleScreenState _titleScreenState;
    [SerializeField] private ActionState _actionState;

    private IGameState _currentState;

    private void Awake()
    {
        _loadingState.OnLoadingCompleted += HandleLoadingCompleted;
        _titleScreenState.OnPlayRequested += HandlePlayRequested;
    }

    private async UniTask Start()
    {
        await UniTask.Yield();
        ConfigSettings();
        await ChangeState(_loadingState);
    }

    private void ConfigSettings()
    {
        Input.multiTouchEnabled = false;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    private void HandleLoadingCompleted() => ChangeState(_titleScreenState).Forget();

    private void HandlePlayRequested() => ChangeState(_actionState).Forget();

    public UniTask GoToTitleScreen() => ChangeState(_titleScreenState);

    private async UniTask ChangeState(IGameState nextState)
    {
        if (_currentState != null) await _currentState.Exit();
        _currentState = nextState;
        await _currentState.Enter();
    }
}
