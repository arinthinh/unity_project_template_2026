using Cysharp.Threading.Tasks;

public interface IGameState
{
    public UniTask Enter();
    public UniTask Exit();
}
