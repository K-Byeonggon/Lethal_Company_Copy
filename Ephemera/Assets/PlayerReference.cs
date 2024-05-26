using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReference : MonoBehaviour
{
    public static PlayerReference Instance;

    [SerializeField]
    Dictionary<uint, PlayerController> playerDic = new Dictionary<uint, PlayerController>();

    public int PlayerCount => playerDic.Count;

    private void Awake()
    {
        Instance = this;
    }

    public void AddPlayerToDic(PlayerController player)
    {
        Instance.playerDic.Add(player.netId, player);
    }

    public void ClearRoom()
    {
        playerDic.Clear();
    }
}
