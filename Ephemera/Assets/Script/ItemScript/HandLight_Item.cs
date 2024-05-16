using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandLight_Item : Item
{
    private bool isActive = true;
    [SerializeField]
    Light flashlight;   

    public void OnClickFireKey(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            UseItem();
        }
        else if(context.performed)
        {

        }
        else if(context.canceled)
        {
            
        }
    }
    public override void UseItem()
    {
        Debug.Log("click");
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
