using UnityEngine;

public class SuccessHit : MonoBehaviour
{
    [SerializeField] private Receiver _receiver;
    [SerializeField] private Renderer _receiverRenderer;
    [SerializeField] private LineRenderer _lineRenderer;
    

    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private Color _hitColor = Color.green;
    private Color _receiverOriginalColor;
    private Color _laserOriginalColor;

    private void Awake()
    {
        _receiverOriginalColor = _receiverRenderer.material.color;
        _laserOriginalColor = _lineRenderer.material.GetColor(EmissionColor);
    }

    private void OnEnable()
    {
        _receiver.OnActivationChanged += SetHitColor;
    }

    private void OnDisable()
    {
        _receiver.OnActivationChanged -= SetHitColor;
    }

    private void SetHitColor(bool isHit)
    {
        // 리시버 색상 변경
        _receiverRenderer.material.color = isHit ? _hitColor : _receiverOriginalColor;
        // 레이저는 Emission 셰이더 색상 변경
        _lineRenderer.material.SetColor(EmissionColor, isHit ? ApplyHue(_laserOriginalColor, _hitColor) : _laserOriginalColor);
    }

    // 밝기 유지
    private static Color ApplyHue(Color original, Color hueSource)
    {
        Color.RGBToHSV(original, out _, out var s, out var v);
        Color.RGBToHSV(hueSource, out var h, out _, out _);
        return Color.HSVToRGB(h, s, v, true);
    }
}
