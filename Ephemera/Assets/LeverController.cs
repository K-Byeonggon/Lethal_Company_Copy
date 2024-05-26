using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LeverController : NetworkBehaviour, IInteractive, IUIVisible
{
    [SerializeField]
    GameObject LeverObject;
    [SerializeField]
    BoxCollider boxCollider;

    Quaternion originRotation = Quaternion.Euler(-51.244f, 0f, 0f);
    Quaternion targetRotation = Quaternion.Euler(-151.897f, 0f, 0f);
    float threshold = 1.0f; // ���� ���� �Ӱ谪 (�ʿ信 ���� ����)

    [Command(requiresAuthority = false)]
    public void OnInteractive()
    {
        if(GameManager.Instance.IsLand == false)
            return;

        LeverLerpStart();
        GameManager.Instance.OnServerEscapePlanet();
    }

    [ClientRpc]
    public void LeverLerpStart()
    {
        StartCoroutine(LeverLerp());
    }

    public IEnumerator LeverLerp()
    {
        boxCollider.enabled = false;
        while (true)
        {
            LeverObject.transform.localRotation = Quaternion.Lerp(LeverObject.transform.localRotation, targetRotation, Time.deltaTime * 5.0f);
            if(Quaternion.Angle(LeverObject.transform.localRotation, targetRotation) < threshold)//���̰� ���ݹۿ� �ȳ��� 
            {
                LeverObject.transform.localRotation = targetRotation;
                break;
            }
            yield return null;
        }
        StartCoroutine(LeverRecovery());
    }
    public IEnumerator LeverRecovery()
    {
        while (true)
        {
            LeverObject.transform.localRotation = Quaternion.Lerp(LeverObject.transform.localRotation, originRotation, Time.deltaTime * 5.0f);
            if (Quaternion.Angle(LeverObject.transform.localRotation, originRotation) < threshold)//���̰� ���ݹۿ� �ȳ��� 
                break;
            yield return null;
        }
        LeverObject.transform.localRotation = originRotation;
        boxCollider.enabled = true;
    }
}
