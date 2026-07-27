using UnityEngine;

// 레이저가 충돌했을 때 처리 방식 결정
public interface IReflective
{
    LaserHitResult OnLaserHit(Vector3 dir, Vector3 normal);
}
