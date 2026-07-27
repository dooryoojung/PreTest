using UnityEngine;

public readonly struct LaserHitResult
{
    public readonly bool ShouldContinue;
    public readonly Vector3 NextDir;

    private LaserHitResult(bool shouldContinue, Vector3 nextDirection)
    {
        ShouldContinue = shouldContinue;
        NextDir = nextDirection;
    }

    public static LaserHitResult Stop() => new(false, Vector3.zero);

    public static LaserHitResult Continue(Vector3 nextDir) => new(true, nextDir);
}
