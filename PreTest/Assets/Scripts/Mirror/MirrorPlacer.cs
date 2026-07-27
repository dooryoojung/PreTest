using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MirrorPlacer : MonoBehaviour
{
    [SerializeField] private GameObject _mirrorPrefab;
    [SerializeField] private Transform _mirrorContainer;
    [SerializeField] private Transform _ground;
    [SerializeField] private Camera _camera;
    
    [SerializeField] private float _rotateStep = 15f;
    [SerializeField] private float _tiltStep = 15f;

    private bool _isPlacing;
    private bool _isDragging;
    
    private readonly List<GameObject> _spawnedMirrors = new();
    
    private MirrorController _selectedController;

    private void Awake()
    {
        if (_camera == null) _camera = Camera.main;
    }

    private void Update()
    {
        HandleRotationInput();

        if (Input.GetMouseButtonDown(0)) HandleMouseDown();
        else if (_isDragging && Input.GetMouseButton(0)) HandleDrag();

        if (Input.GetMouseButtonUp(0)) _isDragging = false;
    }

    public void OnClickAddMirror()
    {
        Deselect();
        _isPlacing = true;
    }

    public void OnClickRemoveMirror()
    {
        if (_selectedController == null) return;

        _spawnedMirrors.Remove(_selectedController.gameObject);
        Destroy(_selectedController.gameObject);
        _selectedController = null;
    }

    public void OnClickReset()
    {
        foreach (var mirror in _spawnedMirrors)
        {
            Destroy(mirror);
        }

        _spawnedMirrors.Clear();
        _selectedController = null;
    }

    private void HandleMouseDown()
    {
        // ui 선택 제외
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (_isPlacing)
        {
            if (TryGetGroundPoint(out var point, out var normal))
            {
                PlaceMirror(point, normal);
                _isPlacing = false;
            }
            return;
        }

        if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out var hit))
        {
            var mirror = hit.collider.GetComponent<Mirror>();
            if (mirror != null)
            {
                var controller = mirror.Root.GetComponent<MirrorController>();
                if (controller == _selectedController)
                {
                    _isDragging = true;
                }
                else
                {
                    Select(controller);
                }
                return;
            }
        }

        Deselect();
    }

    private void HandleDrag()
    {
        if (_selectedController == null) return;

        if (TryGetGroundPoint(out var point, out _))
        {
            _selectedController.MoveTo(point);
        }
    }

    private void HandleRotationInput()
    {
        if (_selectedController == null) return;

        // Q/W: y축 회전
        if (Input.GetKeyDown(KeyCode.Q)) _selectedController.Spin(-_rotateStep);
        if (Input.GetKeyDown(KeyCode.W)) _selectedController.Spin(_rotateStep);

        // E/R: x축 기울이기
        if (Input.GetKeyDown(KeyCode.E)) _selectedController.Tilt(-_tiltStep);
        if (Input.GetKeyDown(KeyCode.R)) _selectedController.Tilt(_tiltStep);
    }

    private void PlaceMirror(Vector3 position, Vector3 groundNormal)
    {
        // 선택한 표면의 법선에 맞춰 수직으로 생성
        var rotation = Quaternion.FromToRotation(Vector3.up, groundNormal);
        var instance = Instantiate(_mirrorPrefab, position, rotation, _mirrorContainer);
        instance.AddComponent<MirrorController>();
        _spawnedMirrors.Add(instance);
    }

    private void Select(MirrorController controller)
    {
        Deselect();

        _selectedController = controller;
        _selectedController.Select();
    }

    private void Deselect()
    {
        if (_selectedController != null) _selectedController.Deselect();

        _selectedController = null;
        _isDragging = false;
    }

    // 실제 바닥 콜라이더에 Raycast -> 벽 관통 막고 바닥에만 배치
    private bool TryGetGroundPoint(out Vector3 point, out Vector3 normal)
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit) && hit.transform.IsChildOf(_ground))
        {
            point = hit.point;
            normal = hit.normal;
            return true;
        }

        point = default;
        normal = Vector3.up;
        return false;
    }
}
