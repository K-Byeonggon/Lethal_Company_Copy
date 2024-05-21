using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IHasInput
{
    //�� �Լ��� OnEnable()�� �����ؾ���
    protected void BindingInputAction(InputType inputType, InputActionHandler handler);
    /* ���� (������ �Լ��� ���)
    {
        InputManager.Instance.BindAction(InputType.OnMove , new InputActionHandler(started �Լ�, performed �Լ�, canceled �Լ�));
    }
    */

    //�� �Լ��� OnDisable()�� �����ؾ���
    protected void UnbindingInputAction(InputType inputType);
    /* ���� (������ �Լ��� ���)
    {
        InputManager.Instance.UnbindAction(InputType.OnMove);

    }
    */
}
