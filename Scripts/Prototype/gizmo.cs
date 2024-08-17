using UnityEngine;

public class TwoSpheresWithRotation : MonoBehaviour
{
    public float sphereRadius = 1.0f;

    private void Start()
    {
        sphereRadius = transform.localScale.x;
    }
    private void OnDrawGizmos()
    {
        // Draw the first sphere at the object's position with no rotation
        DrawSphereGizmo(transform.position, Quaternion.identity, Color.blue);

        // Draw the second sphere with a 45-degree offset rotation
        Quaternion offsetRotation = Quaternion.Euler(0, 45, 0);
        DrawSphereGizmo(transform.position, offsetRotation, Color.blue);
    }

    private void DrawSphereGizmo(Vector3 position, Quaternion rotation, Color color)
    {
        Gizmos.color = color;
        Gizmos.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
        Gizmos.DrawWireSphere(Vector3.zero, sphereRadius);
    }
}
