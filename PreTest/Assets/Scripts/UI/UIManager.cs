using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Laser _laser;
    [SerializeField] private MirrorPlacer _mirrorPlacer;

    [SerializeField] private TMP_Text _reflectCountText;
    [SerializeField] private TMP_Text _mirrorCountText;

    private int _lastReflectCount = -1;
    private int _lastMirrorCount = -1;

    private void Update()
    {
        if (_laser.ReflectCount != _lastReflectCount)
        {
            _lastReflectCount = _laser.ReflectCount;
            _reflectCountText.text = $"반사횟수: {_lastReflectCount}/{Laser.MaxReflections}회";
        }

        if (_mirrorPlacer.MirrorCount != _lastMirrorCount)
        {
            _lastMirrorCount = _mirrorPlacer.MirrorCount;
            _mirrorCountText.text = $"미러갯수: {_lastMirrorCount}/{_mirrorPlacer.MaxMirrorCount}개";
        }
    }
}
