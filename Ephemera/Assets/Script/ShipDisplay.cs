using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShipDisplay : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI DeadLineText;
    [SerializeField]
    TextMeshProUGUI TargetMoneyText;

    private void Start()
    {
        GameManager.Instance.RegistTargetMoneyDisplayAction(ChangeTargetMoneyText);
        GameManager.Instance.RegistDeadLineDisplayAction(ChangeDeadLineText);
    }
    private void ChangeDeadLineText(string day)
    {
        DeadLineText.text = $"DeadLine : {day}";
    }
    private void ChangeTargetMoneyText(string day)
    {
        TargetMoneyText.text = $"Quota : {day}";
    }
}
