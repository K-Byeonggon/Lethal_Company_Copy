using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    bool hasDestination = false;
    Vector3 Destination = Vector3.zero;
    Quaternion lookAt;
    private void FixedUpdate()
    {
        if (hasDestination == true)
        {
            Debug.Log("isLerp");
            transform.rotation = Quaternion.Slerp(transform.rotation, lookAt, 0.01f);
        }
    }

    public void SetDestination()
    {
        Destination = Vector3.zero;
        Vector3 targetDirection = Destination - transform.position;
        lookAt = Quaternion.FromToRotation(transform.forward, targetDirection);
        hasDestination = true;

        Debug.Log(Destination);
        Debug.Log(lookAt);
    }
}
