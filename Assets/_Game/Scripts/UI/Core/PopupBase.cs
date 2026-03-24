using DG.Tweening;
using Redcode.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class PopupBase : UIBase
{
    public override EUILayer UILayer => EUILayer.Popup;

    protected void PlayAppearAnimation(Image backgroundImage, RectTransform contentPanel)
    {
        backgroundImage.SetColorA(0);
        backgroundImage.DOFade(0.9f, 0.3f);

        contentPanel.localScale = Vector3.one * 1.1f;
        contentPanel.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }
}