using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceSystem : NetworkBehaviour
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
        Debug.Log(isDrive);
        if (isSetDirection)
        {
            SlerpShipRotation();
        }
        if(isDrive)
        {
            SlerpPlanetDistance();
        }
    }


    [Server]
    public void StartWarpDrive(int planet)
    {
        SelectPlanet((Planet)planet);
        isDrive = true;
    }

    //행성 방향 회전
    [Server]
    public void SetDirection()
    {
        TargetQuaternion = Quaternion.identity;
        isSetDirection = true;
    }

    //함선 회전 구형보간
    [Server]
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
    [Server]
    public void SelectPlanet(Planet planet)
    {
        Debug.Log(planet.ToString());

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

        Debug.Log(targetPlanet);

        targetPlanet.transform.position = new Vector3(0, 0, 5000);

        PlanetActivate(targetPlanet);// targetPlanet.SetActive(true);
    }
    //행성 거리 구형보간
    [Server]
    public void SlerpPlanetDistance()
    {
        Debug.Log(targetPlanet);

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
    //행성 활성화
    [ClientRpc]
    public void PlanetActivate(GameObject targetPlanet)
    {
        Debug.Log("PlanetActivate");
        targetPlanet.SetActive(true);
    }
    //행성 비활성화
    [ClientRpc]
    public void PlanetDeactivate()
    {
        currentPlanet?.SetActive(false);
    }
}

