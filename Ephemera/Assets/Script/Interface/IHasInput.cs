using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IHasInput
{
    //이 함수는 OnEnable()에 적재해야함
    protected void BindingInputAction(InputType inputType, InputActionHandler handler);
    /* 예시 (움직임 함수의 경우)
    {
        InputManager.Instance.BindAction(InputType.OnMove , new InputActionHandler(started 함수, performed 함수, canceled 함수));
    }
    */

    //이 함수는 OnDisable()에 적재해야함
    protected void UnbindingInputAction(InputType inputType);
    /* 예시 (움직임 함수의 경우)
    {
        InputManager.Instance.UnbindAction(InputType.OnMove);

    }
    */
}
