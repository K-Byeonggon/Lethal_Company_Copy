using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpaceSystem : NetworkBehaviour
{
    public static SpaceSystem Instance;

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

    private void Awake()
    {
        Instance = this;
    }

    private void FixedUpdate()
    {
        //Debug.Log(isDrive);
        /*if (isSetDirection)
        {
            SlerpShipRotation();
        }*/
        if(isDrive)
        {
            SlerpPlanetDistance();
        }
    }

    public override void OnStartClient()
    {
        Instance = this;
        //NetworkServer.RegisterHandler<PlanetActivateMessage>(PlanetSetActivate);
    }

    [Server]
    public bool StartWarpDrive(int planet)
    {
        if (isDrive == true)
            return false;
        SelectPlanet((Planet)planet);
        isDrive = true;
        return true;
    }

    //행성 방향 회전
    [Server]
    public void SetDirection()
    {
        TargetQuaternion = Quaternion.identity;
        isSetDirection = true;
    }

    /*//함선 회전 구형보간
    [Server]
    public void SlerpShipRotation()
    {
        currentPlanet.transform.rotation = Quaternion.Slerp(currentPlanet.transform.rotation, TargetQuaternion, 0.1f);

        if (targetPlanet.transform.position.z < 0.1f)
        {
            Debug.Log("Rotation_End");
            targetPlanet.transform.position = Vector3.zero;
            isDrive = false;
            //PlanetDeactivate();
            currentPlanet = targetPlanet;
        }
    }*/

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

        PlanetActivate(PlanetToEnum(targetPlanet));// targetPlanet);// targetPlanet.SetActive(true);
        //NetworkClient.Send(new PlanetActivateMessage { planet = planet, isActive = true });
    }
    //행성 거리 구형보간
    [Server]
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
            PlanetDeactivate(PlanetToEnum(targetPlanet));
        }
    }
    //행성 활성화
    [ClientRpc]
    public void PlanetActivate(Planet targetPlanet)
    {
        Debug.Log("PlanetActivate");
        EnumToPlanet(targetPlanet).SetActive(true);
        //targetPlanet.SetActive(true);
    }
    //행성 비활성화
    [ClientRpc]
    public void PlanetDeactivate(Planet targetPlanet)
    {
        currentPlanet?.SetActive(false);
        currentPlanet = EnumToPlanet(targetPlanet);
        //currentPlanet = targetPlanet;
    }

    public void SetActivateSpaceSystem(bool isActive)
    {
        this.gameObject.SetActive(isActive);
    }

    /*void PlanetSetActivate(NetworkConnectionToClient conn, PlanetActivateMessage message)
    {
        //message.planet
        EnumToPlanet(message.planet).SetActive(message.isActive);
    }*/
    /*void PlanetDeactivate(NetworkConnectionToClient conn, PlanetActivateMessage message)
    {
        message.targetPlanet.SetActive(message.isActive);
    }*/


    private GameObject EnumToPlanet(Planet planet)
    {
        GameObject go = null;
        switch (planet)
        {
            case Planet.Mars:
                go = Mars;
                break;
            case Planet.Mercury:
                go = Mercury;
                break;
            case Planet.Moon:
                go = Moon;
                break;
            case Planet.Pluto:
                go = Pluto;
                break;
            case Planet.Venus:
                go = Venus;
                break;
        }
        return go;
    }
    private Planet PlanetToEnum(GameObject planet)
    {
        Planet en = Planet.Moon;
        switch (planet.name)
        {
            case "Mars":
                en = Planet.Mars;
                break;
            case "Mercury":
                en = Planet.Mercury;
                break;
            case "Moon":
                en = Planet.Moon;
                break;
            case "Pluto":
                en = Planet.Pluto;
                break;
            case "Venus":
                en = Planet.Venus;
                break;
        }
        return en;
    }
}

