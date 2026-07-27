using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private const int MaxReflectCount = 11;
    
    [SerializeField] private Transform _muzzle;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _maxDistance = 50f;
    
    private List<Vector3> _points = new();

    private void Awake()
    {
        _lineRenderer.useWorldSpace = true;
    }

    private void Update()
    {
        _points.Clear();

        var origin = _muzzle.position;
        var direction = _muzzle.forward;
        
        _points.Add(origin);

        for (var reflect = 0; reflect < MaxReflectCount; reflect++)
        {
            if (!Physics.Raycast(origin, direction, out var hit, _maxDistance))
            {
                _points.Add(origin + direction * _maxDistance);
                break;
            }

            _points.Add(hit.point);

            var reflective = hit.collider.GetComponent<IReflective>();
            if (reflective != null)
            {
                var result = reflective.OnLaserHit(direction, hit.normal);
                if (result.ShouldContinue)
                {
                    direction = result.NextDir;

                    // 반사한 오브젝트와 다시 충돌할 수 있어서 약간 띄움
                    origin = hit.point + direction * 0.001f;
                    continue;
                }
            }

            break;
        }

        _lineRenderer.positionCount = _points.Count;
        _lineRenderer.SetPositions(_points.ToArray());
    }
}
