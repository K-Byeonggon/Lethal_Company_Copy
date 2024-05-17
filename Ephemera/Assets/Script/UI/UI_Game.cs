using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Game : MonoBehaviour
{
    [SerializeField]
    private Slider playerHpBar;
    [SerializeField]
    private Slider playerStaminaBar;
    [SerializeField]
    private UI_Blink settingIcon;
    [SerializeField]
    private UI_Blink missionIcon;
    [SerializeField]
    private List<Image> items;
    [SerializeField]
    private Image weapon;
    [SerializeField]
    private TextMeshProUGUI weaponName;

    private List<Slider> otherHpBars;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ItemSelection(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ItemSelection(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ItemSelection(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ItemSelection(3);
        }
    }

    public void Init()
    {

    }

    //클라이언트 추가, UI hpbar 추가
    public void AddClient()
    {

    }

    //UI 상호작용 깜박임
    public void UIInteraction()
    {

    }


    public void ItemSelection(int index)
    {
        foreach (var item in items)
        {
            if(item == items[index])
            {
                item.color = UI_Color.NormalColor;
            }
            else
            {
                item.color = UI_Color.HideColor;
            }
        }
    }
    public void AddItem(int index, Sprite image)
    {
        items[index].sprite = image;
    }
}
