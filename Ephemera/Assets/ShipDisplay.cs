using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShipDisplay : NetworkBehaviour
{
    
    [SerializeField]
    TextMeshProUGUI DeadLineText;
    [SerializeField]
    TextMeshProUGUI TargetMoneyText;
    public override void OnStartClient()
    {
        GameManager.Instance.RegistTargetMoneyDisplayAction(ChangeTargetMoneyText);
        GameManager.Instance.RegistDeadLineDisplayAction(ChangeDeadLineText);
    }
    public void ChangeDeadLineText(string day)
    {
        DeadLineText.text = $"DeadLine : {day}";
    }
    public void ChangeTargetMoneyText(string day)
    {
        TargetMoneyText.text = $"Quota : {day}";
    }
}
