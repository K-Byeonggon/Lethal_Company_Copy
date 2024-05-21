using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OtherPlayerStatus : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI playerName;
    [SerializeField]
    TextMeshProUGUI stateText;
    [SerializeField]
    Slider playerHpbar;

    public TextMeshProUGUI PlayerName {  get { return playerName; } }
    public TextMeshProUGUI StateText {  get { return stateText; } }
    public Slider PlayerHpbar { get { return playerHpbar; } }
}
