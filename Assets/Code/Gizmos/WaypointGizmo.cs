using UnityEngine;

public class WaypointGizmo : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta; // Set gizmo color
        Gizmos.DrawSphere(transform.position, 0.2f); // Draw sphere at position
    }
}
