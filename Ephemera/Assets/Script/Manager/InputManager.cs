
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : SingleTon<InputManager>
{
    PlayerInput playerInput;
    public PlayerInput PlayerInput
    {
        get 
        { 
            if (playerInput == null)
            {
                GameObject go = Instantiate(ResourceManager.Instance.GetPrefab("InputSystem"));
                go.transform.SetParent(transform, false);
                playerInput = go.GetComponent<PlayerInput>();
            }
            return playerInput;
        }
    }
}
