using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private const int MaxReflectCount = 11;
    
    [SerializeField] private Transform _muzzle;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _maxDistance = 50f;
    
    private readonly List<Vector3> _points = new();

    private void Awake()
    {
        _lineRenderer.useWorldSpace = true;
    }

    private void Update()
    {
        LaserPath.Compute(
            _muzzle.position,
            _muzzle.forward,
            _maxDistance,
            MaxReflectCount,
            _points);

        _lineRenderer.positionCount = _points.Count;
        _lineRenderer.SetPositions(_points.ToArray());
    }
}
