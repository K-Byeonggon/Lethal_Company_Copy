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

    public static TerrainController Instance;

    private void Awake()
    {
        Instance = this;
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
}