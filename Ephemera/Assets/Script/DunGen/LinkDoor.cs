using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkDoor : MonoBehaviour, IUIVisible
{
    public Transform LinkTransform;

    private void Start()
    {
        Debug.Log("StartLinkDoor");
    }
    public Vector3 GetTeleportionPosition()
    {
        return LinkTransform.position + LinkTransform.forward * 2;
    }

    /*public void OnInteractive(Transform position)
    {

    }*/
}
