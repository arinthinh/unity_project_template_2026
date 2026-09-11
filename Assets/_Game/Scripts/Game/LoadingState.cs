using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadingState : MonoBehaviour, IGameState
{
    [SerializeField] private float _loadingDuration = 2f;
    
    public void Enter()
    {
    }

    public void Exit()
    {
    }
}