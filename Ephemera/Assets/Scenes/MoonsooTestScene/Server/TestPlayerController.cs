using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerController : MonoBehaviour
{
    public void OnMove(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            Vector2 vec = context.ReadValue<Vector2>();
            transform.position = new Vector3(vec.x, 0, vec.y);
        }
        else if(context.performed)
        {

        }
        else if(context.canceled)
        {

        }
    }

}
