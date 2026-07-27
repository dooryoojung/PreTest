using System.Collections.Generic;
using UnityEngine;

public static class LaserPath
{
    public static void Compute(
        Vector3 origin,
        Vector3 direction,
        float maxDistance,
        int maxSegments,
        List<Vector3> result,
        out int reflectionCount)
    {
        result.Clear();
        result.Add(origin);
        reflectionCount = 0;

        for (var segment = 0; segment < maxSegments; segment++)
        {
            if (!Physics.Raycast(origin, direction, out var hit, maxDistance))
            {
                result.Add(origin + direction * maxDistance);
                return;
            }

            result.Add(hit.point);

            var reflective = hit.collider.GetComponent<IReflective>();
            if (reflective != null)
            {
                var reflectResult = reflective.OnLaserHit(direction, hit.normal);
                if (reflectResult.ShouldContinue)
                {
                    reflectionCount++;
                    direction = reflectResult.NextDir;

                    // 반사한 오브젝트와 다시 충돌할 수 있어서 약간 띄움
                    origin = hit.point + direction * 0.001f;
                    continue;
                }
            }

            return;
        }
    }
}
