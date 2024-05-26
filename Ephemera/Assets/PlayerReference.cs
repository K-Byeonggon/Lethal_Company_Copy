using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReference : MonoBehaviour
{
    public static PlayerReference Instance;

    [SerializeField]
    Dictionary<uint, PlayerHealth> playerDic = new Dictionary<uint, PlayerHealth>();

    public int PlayerCount => playerDic.Count;

    private void Awake()
    {
        Instance = this;
    }

    public void AddPlayerToDic(PlayerHealth player)
    {
        if (playerDic.ContainsValue(player) == false)
            Instance.playerDic.Add(player.netId, player);
    }
    public void RemovePlayerToDic(PlayerHealth player)
    {
        if (playerDic.ContainsValue(player))
            Instance.playerDic.Remove(player.netId);
    }
    public void RemovePlayerToDic(uint netId)
    {
        if (playerDic.ContainsKey(netId))
            Instance.playerDic.Remove(netId);
    }

    public void ClearRoom()
    {
        playerDic.Clear();
    }
}
