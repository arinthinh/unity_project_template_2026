using Cysharp.Threading.Tasks;
using UnityEngine;

public class ActionState : MonoBehaviour, IGameState
{
    public UniTask Enter() => UniTask.CompletedTask;

    public UniTask Exit() => UniTask.CompletedTask;
}
