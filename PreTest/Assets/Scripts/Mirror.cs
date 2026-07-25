using UnityEngine;

public class Mirror : MonoBehaviour
{
    public Vector3 Reflect(Vector3 dir, Vector3 normal)
    {
        return Vector3.Reflect(dir, normal);
    }
}
