using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TitleScreenState : MonoBehaviour, IGameState
{
    public event Action OnPlayRequested;

    public UniTask Enter()
    {
        TitleScreenData data = new();
        data.OnPlayButtonClicked += HandlePlayButtonClicked;
        data.OnExitButtonClicked += HandleExitButtonClicked;
        UIManager.Instance.ShowScreen<TitleScreen>(data);
        return UniTask.CompletedTask;
    }

    public UniTask Exit()
    {
        UIManager.Instance.HideScreen<TitleScreen>();
        return UniTask.CompletedTask;
    }

    private void HandlePlayButtonClicked() => OnPlayRequested?.Invoke();

    private void HandleExitButtonClicked() => Application.Quit();
}
