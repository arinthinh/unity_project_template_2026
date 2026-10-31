using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : ScreenBase
{
    [Header("BUTTONS")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _exitButton;

    private TitleScreenData _data;

    public override void OnInit(UIManager uiManager)
    {
        base.OnInit(uiManager);
        _playButton.onClick.AddListener(HandlePlayButtonClicked);
        _exitButton.onClick.AddListener(HandleExitButtonClicked);
    }

    public override void Show(object data = null)
    {
        _data = (TitleScreenData)data;
        base.Show(data);
    }

    public override void Hide()
    {
        base.Hide();
        _data = null;
    }

    private void HandlePlayButtonClicked() => _data.InvokePlayButtonClicked();

    private void HandleExitButtonClicked() => _data.InvokeExitButtonClicked();
}
