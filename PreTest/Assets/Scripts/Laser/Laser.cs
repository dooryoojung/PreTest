using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // 반사 최대 10회
    public const int MaxReflections = 10;
    private const int MaxSegments = 11;

    [SerializeField] private Transform _muzzle;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _maxDistance = 50f;
    
    private readonly List<Vector3> _points = new();

    public int ReflectCount { get; private set; }

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
            MaxSegments,
            _points,
            out var reflectCount);

        ReflectCount = reflectCount;

        _lineRenderer.positionCount = _points.Count;
        _lineRenderer.SetPositions(_points.ToArray());
    }
}
