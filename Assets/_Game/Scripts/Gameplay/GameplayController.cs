using Cysharp.Threading.Tasks;
using Redcode.Pools;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    // Services
    private UIManager _uiManager;

    private void Start() => Run().Forget();

    private async UniTaskVoid Run()
    {
        _uiManager = UIManager.Instance;
        await UniTask.Yield();
    }
}