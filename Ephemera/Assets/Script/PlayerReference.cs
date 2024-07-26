using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerReference : MonoBehaviour
{
    public static PlayerReference Instance;

    public PlayerHealth localPlayer;

    [SerializeField]
    public Dictionary<uint, PlayerHealth> playerDic = new Dictionary<uint, PlayerHealth>();

    public int PlayerCount => playerDic.Count;

    private void Awake()
    {
        Instance = this;
    }
    public void AddLocalPlayer(PlayerHealth player)
    {
        localPlayer = player;
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
    public int GetPlayerOrder(uint playerKey)
    {
        // Dictionary�� Ű�� �����մϴ�.
        var sortedKeys = playerDic.Keys.OrderBy(key => key).ToList();

        // �ڽ��� Ű�� �� ��°�� �ִ��� ã���ϴ�.
        int index = sortedKeys.IndexOf(playerKey);

        // 0���� �����ϴ� �ε����� 1���� �����ϴ� ������ ��ȯ�մϴ�.
        return index;
    }

    public void ClearRoom()
    {
        playerDic.Clear();
    }
}
