using UnityEngine;

public class MirrorController : MonoBehaviour
{
    private static readonly Color SelectedColor = Color.black;

    private Renderer _renderer;
    private Color _originalColor;

    private void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
        if (_renderer != null)
        {
            _originalColor = _renderer.material.color;
        }
    }

    public void Select()
    {
        if (_renderer != null) _renderer.material.color = SelectedColor;
    }

    public void Deselect()
    {
        if (_renderer != null) _renderer.material.color = _originalColor;
    }

    public void MoveTo(Vector3 position)
    {
        transform.position = position;
    }

    // 자신의 y축 기준 회전
    public void Spin(float degrees)
    {
        transform.Rotate(Vector3.up, degrees, Space.Self);
    }

    // x축 기울이기
    public void Tilt(float degrees)
    {
        transform.Rotate(Vector3.right, degrees, Space.Self);
    }
}
