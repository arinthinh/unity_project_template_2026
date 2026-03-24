using UnityEngine;
using UnityEngine.UI;

public class ScrollUVImage : MonoBehaviour
{
    [SerializeField] private bool _isRawImage;
    [SerializeField] private float _x, _y;

    private Image _img;
    private Material _imgMaterial;
    private RawImage _rawImg;

    private void Awake()
    {
        if (_isRawImage)
        {
            _rawImg = this.GetComponent<RawImage>();
        }
        else
        {
            _img = this.GetComponent<Image>();
            _imgMaterial = Instantiate(_img.material);
        }
    }


    private void FixedUpdate()
    {
        if (_isRawImage)
            _rawImg.uvRect = new Rect(_rawImg.uvRect.position + new Vector2(_x, _y) * Time.fixedDeltaTime, _rawImg.uvRect.size);
        else
        {
            _imgMaterial.mainTextureOffset += new Vector2(_x, _y) * Time.fixedDeltaTime;
            _img.material = _imgMaterial;
        }
    }
}