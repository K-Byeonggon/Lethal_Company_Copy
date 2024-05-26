using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class UI_Game : MonoBehaviour
{
    [SerializeField]
    private Slider playerHpBar;
    [SerializeField]
    private Slider playerStaminaBar;
    //[SerializeField]
    //private UI_Blink settingIcon;
    //[SerializeField]
    //private UI_Blink missionIcon;
    //[SerializeField]
    //private List<OtherPlayerStatus> OtherPlayers;
    [SerializeField]
    private List<Image> items;
    //[SerializeField]
    //private Image weapon;
    //[SerializeField]
    //private TextMeshProUGUI weaponName;

    [SerializeField]
    private GameObject interactionImage;

    [SerializeField]
    private List<UI_SpriteSetup> uISpriteSetups;

    [SerializeField]
    private TextMeshProUGUI monnyText;

    private List<Slider> otherHpBars;

    private bool isActive = false;

    private void Start()
    {
        Init();
        GameManager.Instance.RegistCurrentMoneyDisplayAction(CurrentMonnyChangeEvent);
    }
    private void Update()
    {
        if (isActive == false)
            return;
    }
    public void Init()
    {
        if (uISpriteSetups == null)
            return;
        foreach (var uISpriteSetup in uISpriteSetups)
        {
            uISpriteSetup?.OnSetup();
        }
    }
    //Ŭ���̾�Ʈ �߰�, UI hpbar �߰�
    public void AddClient()
    {

    }
    //UI ��ȣ�ۿ� ǥ��
    public void UIInteraction(bool isActive)
    {
        interactionImage.SetActive(isActive);
    }
    public void SetUp()
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
    public void CurrentMonnyChangeEvent(string value)
    {
        monnyText.text = $"Monny : {value}";
    }
}
