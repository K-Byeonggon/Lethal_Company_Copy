using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Selecter : MonoBehaviour
{
    [SerializeField]
    List<Image> images;

    [SerializeField]
    TextMeshProUGUI planetInfoTitle;
    [SerializeField]
    TextMeshProUGUI planetInfoContext;

    int currentSelection = 0;

    private void Start()
    {
        SelectPlanet(currentSelection);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnPressUp();
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnPressDown();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            OnStartHyperDrive();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            OnStartLandPlanet();
        }
    }

    public void OnPressUp()
    {
        if(currentSelection > 0)
            currentSelection--;
        SelectPlanet(currentSelection);
    }
    public void OnPressDown()
    {
        if (currentSelection < images.Count - 1)
            currentSelection++;
        SelectPlanet(currentSelection);
    }
    private void SelectPlanet(int index)
    {
        foreach (Image image in images)
        {
            if(images.IndexOf(image) == index)
            {
                image.color = UI_Color.NormalColor;
            }
            else
            {
                image.color = UI_Color.HideColor;
            }
        }
        DisplayPlanetInfo(index);
    }
    private void DisplayPlanetInfo(int index)
    {
        planetInfoTitle.text = $"1";
        planetInfoContext.text = $"{index}";
    }

    public void OnStartHyperDrive()
    {
        GameManager.Instance.OnStartHyperDrive(currentSelection);
    }
    public void OnStartLandPlanet()
    {
        GameManager.Instance.StartGame();
    }
}
