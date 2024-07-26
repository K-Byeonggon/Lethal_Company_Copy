using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{

    private void Start()
    {
        //InputManager.Instance.BindAction(InputType.OnMove, new InputActionHandler(OnMove, null, null));
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("OnMove");
    }
}
