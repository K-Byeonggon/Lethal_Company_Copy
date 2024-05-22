using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceSystem : MonoBehaviour
{
    //�Լ�
    [SerializeField]
    GameObject ship;

    //�༺
    [SerializeField]
    GameObject Mars;

    //����
    [SerializeField]
    GameObject Mercury;

    //����
    [SerializeField]
    GameObject Moon;

    //����
    [SerializeField]
    GameObject Pluto;

    //����
    [SerializeField]
    GameObject Venus;

    bool isSetDirection = false;
    bool isDrive = false;

    GameObject currentPlanet;

    GameObject targetPlanet;

    Quaternion TargetQuaternion;

    private void FixedUpdate()
    {
        if(isSetDirection)
        {
            SlerpShipRotation();
        }
        if(isDrive)
        {
            SlerpPlanetDistance();
        }
    }



    public void StartWarpDrive(int planet)
    {

        PlanetActivate((Planet)planet);
    }

    //�༺ ���� ȸ��
    public void SetDirection()
    {
        TargetQuaternion = Quaternion.identity;
        isSetDirection = true;
    }
    //�Լ� ȸ�� ��������
    public void SlerpShipRotation()
    {
        currentPlanet.transform.rotation = Quaternion.Slerp(currentPlanet.transform.rotation, TargetQuaternion, 0.1f);

        if (targetPlanet.transform.position.z < 0.1f)
        {
            Debug.Log("Rotation_End");
            targetPlanet.transform.position = Vector3.zero;
            isDrive = false;
            PlanetDeactivate();
            currentPlanet = targetPlanet;
        }
    }

    //�༺ Ȱ��ȭ
    public void PlanetActivate(Planet planet)
    {
        switch (planet)
        {
            case Planet.Mars:
                targetPlanet = Mars;
                break;
            case Planet.Mercury:
                targetPlanet = Mercury;
                break;
            case Planet.Moon:
                targetPlanet = Moon;
                break;
            case Planet.Pluto:
                targetPlanet = Pluto;
                break;
            case Planet.Venus:
                targetPlanet = Venus;
                break;
        }

        targetPlanet.transform.position = new Vector3(0, 0, 5000);

        targetPlanet?.SetActive(true);
        isDrive = true;
    }
    //�༺ �Ÿ� ��������
    public void SlerpPlanetDistance()
    {
        targetPlanet.transform.position = Vector3.Slerp(targetPlanet.transform.position, Vector3.zero, 0.1f);
        if(currentPlanet != null)
            currentPlanet.transform.position = targetPlanet.transform.position - new Vector3(0, 0, 5000);
        if (targetPlanet.transform.position.z < 0.1f)
        {
            Debug.Log("Warp_End");
            targetPlanet.transform.position = Vector3.zero;
            isDrive = false;
            PlanetDeactivate();
            currentPlanet = targetPlanet;
        }
    }
    //�༺ ��Ȱ��ȭ
    public void PlanetDeactivate()
    {
        currentPlanet?.SetActive(false);
    }
}

