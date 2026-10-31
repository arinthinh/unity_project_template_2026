using System;

public class TitleScreenData
{
    public event Action OnPlayButtonClicked;
    public event Action OnExitButtonClicked;

    public void InvokePlayButtonClicked() => OnPlayButtonClicked?.Invoke();
    public void InvokeExitButtonClicked() => OnExitButtonClicked?.Invoke();
}
