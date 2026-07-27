using UnityEngine;

public class Mirror : MonoBehaviour, IReflective
{
    public Transform Root => transform.parent != null ? transform.parent : transform;

    public LaserHitResult OnLaserHit(Vector3 dir, Vector3 normal)
    {
        return LaserHitResult.Continue(Vector3.Reflect(dir, normal));
    }
}
