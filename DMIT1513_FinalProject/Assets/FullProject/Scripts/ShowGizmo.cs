using UnityEngine;

public class ShowGizmo : MonoBehaviour
{
    public float size = 0.1f;
    public Color color = Color.red;

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, size);
    }
}