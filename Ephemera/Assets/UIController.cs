using Org.BouncyCastle.Asn1.X509.Qualified;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField]
    UI_Selecter ui_Selecter;
    [SerializeField]
    UI_Setup ui_Setup;
    [SerializeField]
    UI_Game ui_Game;
    [SerializeField]
    UI_Setting ui_Setting;
    [SerializeField]
    UI_Mission ui_Mission;
    [SerializeField]
    UI_Alarm ui_Alarm;

    private Dictionary<Type, GameObject> uiDictionary;

    private void Awake()
    {
        uiDictionary = new Dictionary<Type, GameObject>
        {
            { typeof(UI_Selecter), ui_Selecter.gameObject },
            { typeof(UI_Setup), ui_Setup.gameObject },
            { typeof(UI_Game), ui_Game.gameObject },
            { typeof(UI_Setting), ui_Setting.gameObject },
            { typeof(UI_Mission), ui_Mission.gameObject },
            { typeof(UI_Alarm), ui_Alarm.gameObject }
        };
    }

    public void SetActivateUI(Type type)
    {
        foreach (var ui in uiDictionary)
        {
            if (type == null)
                ui.Value.SetActive(false);
            else
            {
                if (ui.Key == type)
                    ui.Value.SetActive(true);
                else
                    ui.Value.SetActive(false);
            }
                
        }
    }
}
