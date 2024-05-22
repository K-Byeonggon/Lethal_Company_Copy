using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainController : MonoBehaviour
{
    [SerializeField]
    public Transform shipStartTransform;
    [SerializeField]
    List<PlanetTerrain> planetTerrain = new List<PlanetTerrain>();

    private void Awake()
    {
        GameManager.Instance.TerrainController = this;
    }

    public void SetActivePlanetTerrain(Planet planet, bool isActive = true)
    {
        foreach (var item in planetTerrain)
        {
            if (item.planet == planet)
                item.Terrain.SetActive(isActive);
            else
                item.Terrain.SetActive(false);
        }
    }
    public Transform GetLandingZone(Planet planet)
    {
        foreach (var item in planetTerrain)
        {
            if (item.planet == planet)
                return item.LandingZone;
        }
        return null;
    }
    public GameObject GetFrontDoor(Planet planet)
    {
        foreach (var item in planetTerrain)
        {
            if (item.planet == planet)
                return item.frontDoor;
        }
        return null;
    }
    public GameObject GetBackDoor(Planet planet)
    {
        foreach (var item in planetTerrain)
        {
            if (item.planet == planet)
                return item.backDoor;
        }
        return null;
    }
    public int GetTerrainCount()
    {
        return planetTerrain.Count;
    }
}

[Serializable]
public class PlanetTerrain
{
    public Planet planet;
    public GameObject Terrain;
    public Transform LandingZone;
    public GameObject frontDoor;//TerrainDoor frontDoor;
    public GameObject backDoor; //TerrainDoor backDoor;
}