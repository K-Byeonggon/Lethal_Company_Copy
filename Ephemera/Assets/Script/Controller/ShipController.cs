using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShipController : NetworkBehaviour
{
    bool hasDestination = false;
    //Vector3 Destination = Vector3.zero;
    Quaternion lookAt;

    bool landingPlanet = false;

    public void StartLanding(Vector3 destination)
    {
        // 대상 위치에서 현재 위치를 빼서 방향 벡터 계산
        Vector3 direction = destination - transform.position;

        direction.y = 0;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
        StartCoroutine(Landing(destination));
    }

    IEnumerator Landing(Vector3 destination)
    {
        while (true)
        {
            if(Vector3.Distance(transform.position, destination) < 0.1f)
            {
                transform.position = destination;
                GameManager.Instance.OnServerActiveLocalPlayerCamera();
                yield break;
            }
            //transform.rotation = Quaternion.Slerp(transform.rotation, lookAt, 0.01f);
            transform.position = Vector3.Slerp(transform.position, destination, 0.01f);
            yield return null;
        }
    }
}
