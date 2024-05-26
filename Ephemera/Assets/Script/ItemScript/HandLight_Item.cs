using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandLight_Item : Item
{
    private bool isActive = true;
    [SerializeField]
    Light flashlight;


    [ClientRpc]
    public override void UseItem()
    {
        if (isActive == true)
        {
            Debug.Log("Off");
            flashlight.gameObject.SetActive(false);
            isActive = false;
        }
        else
        {
            Debug.Log("On");
            flashlight.gameObject.SetActive(true);
            isActive = true;
        }
    }

}
