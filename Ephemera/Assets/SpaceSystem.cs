using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceSystem : MonoBehaviour
{
    //함선
    [SerializeField]
    GameObject ship;

    //행성
    [SerializeField]
    GameObject Mars;

    //위성
    [SerializeField]
    GameObject Mercury;

    //위성
    [SerializeField]
    GameObject Moon;

    //위성
    [SerializeField]
    GameObject Pluto;

    //위성
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

    //행성 방향 회전
    public void SetDirection()
    {
        TargetQuaternion = Quaternion.identity;
        isSetDirection = true;
    }
    //함선 회전 구형보간
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

    //행성 활성화
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
    //행성 거리 구형보간
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
    //행성 비활성화
    public void PlanetDeactivate()
    {
        currentPlanet?.SetActive(false);
    }
}

