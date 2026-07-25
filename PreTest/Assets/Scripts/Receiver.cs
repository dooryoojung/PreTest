using System;
using UnityEngine;

public class Receiver : MonoBehaviour
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
        Debug.Log("리시버 충돌");
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
