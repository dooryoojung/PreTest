using System;
using UnityEngine;

public class Receiver : MonoBehaviour, IReflective
{
    // 이번 프레임에 레이저가 닿았는지
    private bool _isHitFrame;
    // 현재 레이저가 닿은 상태인지 확인
    private bool _isHit;

    // 활성 상태 알림 이벤트
    public event Action<bool> OnActivationChanged;
    
    public void HitReceiver()
    {
        _isHitFrame = true;
    }

    public LaserHitResult OnLaserHit(Vector3 dir, Vector3 normal)
    {
        HitReceiver();
        return LaserHitResult.Stop();
    }

    private void LateUpdate()
    {
        // 현재 상태와 이전 상태 비교
        if (_isHitFrame != _isHit)
        {
            _isHit = _isHitFrame;
            OnActivationChanged?.Invoke(_isHit);
        }

        _isHitFrame = false;
    }
}
