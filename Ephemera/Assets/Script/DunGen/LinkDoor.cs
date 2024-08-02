using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkDoor : MonoBehaviour, IUIVisible
{
    public Transform LinkTransform;

    public Vector3 GetTeleportionPosition()
    {
        return LinkTransform.position + LinkTransform.forward * 2;
    }

    /*public void OnInteractive(Transform position)
    {

    }*/
}
