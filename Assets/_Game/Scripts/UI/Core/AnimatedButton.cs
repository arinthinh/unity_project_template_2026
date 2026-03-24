using DG.Tweening;
using Redcode.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimatedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float _scaleUpFactor = 1.1f;
    [SerializeField] private float _scaleDownFactor = 0.9f;
    [SerializeField] private bool _playIdleAnim;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        if (_playIdleAnim)
        {
            _rectTransform.DOScale(_scaleUpFactor, 1f)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void OnDisable()
    {
        _rectTransform.DOKill();
        _rectTransform.SetLocalScale(1);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _rectTransform.DOKill();
        _rectTransform.DOScale(_scaleDownFactor, 0.1f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _rectTransform.DOKill();
        _rectTransform.DOScale(1f, 0.1f)
            .OnComplete(() =>
            {
                if (_playIdleAnim)
                    _rectTransform.DOScale(_scaleUpFactor, 1f)
                        .SetLoops(-1, LoopType.Yoyo);
            });
    }
}