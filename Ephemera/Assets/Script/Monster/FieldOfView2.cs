using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView2 : MonoBehaviour
{
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 90f;
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    private void OnDrawGizmos()
    {
        DrawFieldOfView();
    }

    private void DrawFieldOfView()
    {
        Gizmos.color = Color.yellow;
        Vector3 startPosition = transform.position;

        // Draw the field of view lines
        for (int i = 0; i <= viewAngle; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + (viewAngle * i / viewAngle);
            Vector3 direction = DirectionFromAngle(angle, false);
            Gizmos.DrawLine(startPosition, startPosition + direction * viewRadius);
        }

        // Optionally, draw an arc representing the field of view
        float halfFOV = viewAngle * 0.5f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(startPosition, startPosition + leftRayDirection * viewRadius);
        Gizmos.DrawLine(startPosition, startPosition + rightRayDirection * viewRadius);

        // Draw the arc
        Gizmos.color = Color.green;
        int segments = 10;
        float angleStep = viewAngle / segments;

        Vector3 previousPoint = startPosition + leftRayDirection * viewRadius;
        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = -halfFOV + angleStep * i;
            Vector3 currentDirection = Quaternion.AngleAxis(currentAngle, Vector3.up) * transform.forward;
            Vector3 currentPoint = startPosition + currentDirection * viewRadius;
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
    }

    private Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}