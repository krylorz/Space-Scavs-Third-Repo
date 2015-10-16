using UnityEngine;
using System.Collections;

public class RoomConnection : MonoBehaviour
{
    [HideInInspector]
    public RoomConnection partner;
    void OnDrawGizmos()
    {
        float scale = 1.0f;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position - transform.right * scale);
        Gizmos.DrawLine(transform.position, transform.position + transform.right * scale);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * scale);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.125f);
    }
}
