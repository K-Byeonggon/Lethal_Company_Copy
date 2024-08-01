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
    public void InitLocalPlayer(PlayerHealth player)
    {
        localPlayer = player;
        GameManager.Instance.OnPlayerLoadedInit(player.connectionToClient);
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
        // Dictionary의 키를 정렬합니다.
        var sortedKeys = playerDic.Keys.OrderBy(key => key).ToList();

        // 자신의 키가 몇 번째에 있는지 찾습니다.
        int index = sortedKeys.IndexOf(playerKey);

        // 0부터 시작하는 인덱스를 1부터 시작하는 순서로 반환합니다.
        return index;
    }

    public void ClearRoom()
    {
        playerDic.Clear();
    }
}
