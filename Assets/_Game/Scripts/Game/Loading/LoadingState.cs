using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadingState : MonoBehaviour, IGameState
{
    [Header("SETTINGS")]
    [SerializeField] private float _loadingDuration = 1f;

    public event Action OnLoadingCompleted;

    // Fire-and-forget so Enter() returns first; raising the event inside Enter would start the next transition mid-transition.
    public UniTask Enter()
    {
        RunLoading().Forget();
        return UniTask.CompletedTask;
    }

    public UniTask Exit() => UniTask.CompletedTask;

    private async UniTask RunLoading()
    {
        await UniTask.WaitForSeconds(_loadingDuration, cancellationToken: destroyCancellationToken);
        OnLoadingCompleted?.Invoke();
    }
}
