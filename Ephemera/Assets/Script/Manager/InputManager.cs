
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : SingleTon<InputManager>
{
    Dictionary<InputType, InputActionHandler> ActionDic = new Dictionary<InputType, InputActionHandler>();

    private void Awake()
    {
        foreach (InputType item in Enum.GetValues(typeof(InputType)))
        {
            ActionDic.Add(item, null);
        }
    }

    public void BindAction(InputType inputType, InputActionHandler inputActionHandler)
    {
        if (ActionDic.ContainsKey(inputType))
        {
            ActionDic[inputType] = inputActionHandler;
        }
        else
        {
            ActionDic.Add(inputType, inputActionHandler);
        }
    }
    public void UnbindAction(InputType inputType)
    {
        if (ActionDic.ContainsKey(inputType))
        {
            ActionDic[inputType] = null;
        }
        else
        {
            ActionDic.Add(inputType, null);
        }
    }

    #region InputCallback Function
    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log($"Move state: {context.phase}");

        if (context.started)
        {
            ActionDic[InputType.OnMove]?.InvokeStarted(context);
        }
        else if (context.performed)
        {
            ActionDic[InputType.OnMove]?.InvokePerformed(context);
        }
        else if (context.canceled)
        {
            ActionDic[InputType.OnMove]?.InvokeCanceled(context);
        }
    }
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ActionDic[InputType.OnRun]?.InvokeStarted(context);
        }
        else if (context.performed)
        {
            ActionDic[InputType.OnRun]?.InvokePerformed(context);
        }
        else if (context.canceled)
        {
            ActionDic[InputType.OnRun]?.InvokeCanceled(context);
        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ActionDic[InputType.OnJump]?.InvokeStarted(context);
        }
        else if (context.performed)
        {
            ActionDic[InputType.OnJump]?.InvokePerformed(context);
        }
        else if (context.canceled)
        {
            ActionDic[InputType.OnJump]?.InvokeCanceled(context);
        }
    }
    public void OnChangItem(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ActionDic[InputType.OnChangItem]?.InvokeStarted(context);
        }
        else if (context.performed)
        {
            ActionDic[InputType.OnChangItem]?.InvokePerformed(context);
        }
        else if (context.canceled)
        {
            ActionDic[InputType.OnChangItem]?.InvokeCanceled(context);
        }
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ActionDic[InputType.OnLook]?.InvokeStarted(context);
        }
        else if (context.performed)
        {
            ActionDic[InputType.OnLook]?.InvokePerformed(context);
        }
        else if (context.canceled)
        {
            ActionDic[InputType.OnLook]?.InvokeCanceled(context);
        }
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ActionDic[InputType.OnAttack]?.InvokeStarted(context);
        }
        else if (context.performed)
        {
            ActionDic[InputType.OnAttack]?.InvokePerformed(context);
        }
        else if (context.canceled)
        {
            ActionDic[InputType.OnAttack]?.InvokeCanceled(context);
        }
    }
    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ActionDic[InputType.OnInteraction]?.InvokeStarted(context); 
        }
        else if (context.performed)
        {
            ActionDic[InputType.OnInteraction]?.InvokePerformed(context);
        }
        else if (context.canceled)
        {
            ActionDic[InputType.OnInteraction]?.InvokeCanceled(context);
        }
    }
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ActionDic[InputType.OnCrouch]?.InvokeStarted(context);
        }
        else if (context.performed)
        {
            ActionDic[InputType.OnCrouch]?.InvokePerformed(context);
        }
        else if (context.canceled)
        {
            ActionDic[InputType.OnCrouch]?.InvokeCanceled(context);
        }
    }
    #endregion
}
public class InputActionHandler
{
    private event Action<InputAction.CallbackContext> started = null;
    private event Action<InputAction.CallbackContext> performed = null;
    private event Action<InputAction.CallbackContext> canceled = null;

    public bool HasStarted()
    {
        return started != null;
    }

    public InputActionHandler(Action<InputAction.CallbackContext> started, Action<InputAction.CallbackContext> performed, Action<InputAction.CallbackContext> canceled)
    {
        this.started = started;
        this.performed = performed;
        this.canceled = canceled;
    }
    public void InvokeStarted(InputAction.CallbackContext context)
    {
        started?.Invoke(context);
    }
    public void InvokePerformed(InputAction.CallbackContext context)
    {
        performed?.Invoke(context);
    }
    public void InvokeCanceled(InputAction.CallbackContext context)
    {
        canceled?.Invoke(context);
    }
}
