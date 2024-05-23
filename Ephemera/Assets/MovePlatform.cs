using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        DependPlatform(other.transform);
    }
    private void OnTriggerExit(Collider other)
    {
        UndependPlatform(other.transform);
    }

    [Server]
    public void DependPlatform(Transform transform)
    {
        SetParent(transform);
    }
    [Server]
    public void UndependPlatform(Transform transform)
    {
        SetParent(null);
    }

    [ClientRpc]

    public void SetParent(Transform transform)
    {
        transform.SetParent(this.transform.parent);
    }
}
