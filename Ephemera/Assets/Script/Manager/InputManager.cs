
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : SingleTon<InputManager>
{
    /*Dictionary<InputType, Action> ActionDic = new Dictionary<InputType, Action>();

    private void Awake()
    {
        foreach (InputType item in Enum.GetValues(typeof(InputType)))
        {
            ActionDic.Add(item, null);
        }
    }

    public void BindAction<T>(InputType inputType, Action inputActionHandler)
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
*/

}