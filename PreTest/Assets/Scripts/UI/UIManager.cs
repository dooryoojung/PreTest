using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Laser _laser;
    [SerializeField] private MirrorPlacer _mirrorPlacer;

    [SerializeField] private TMP_Text _reflectCountText;
    [SerializeField] private TMP_Text _mirrorCountText;

    private void Update()
    {
        _reflectCountText.text = $"반사횟수: {_laser.ReflectCount}/{Laser.MaxReflections}회";
        _mirrorCountText.text = $"미러갯수: {_mirrorPlacer.MirrorCount}/{_mirrorPlacer.MaxMirrorCount}개";
    }
}
