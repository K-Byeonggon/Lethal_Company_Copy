using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPlayer : NetworkRoomPlayer
{
    GameObject roomCharacter;


    #region Command
    [Command]
    void CmdRoomInfoInit()
    {
        Debug.Log(connectionToClient.identity.netId);
        roomCharacter = Instantiate(ResourceManager.Instance.GetPrefab("LobbyScavenger"));
        NetworkServer.Spawn(roomCharacter, connectionToClient);
        TargetRoomInfoInit(connectionToClient);
    }
    [Command]
    public void CmdRoomCharacterInfoChange()
    {

    }
    #endregion

    #region TargetRpc
    [TargetRpc]
    public void TargetRoomInfoInit(NetworkConnection conn)
    {
        
    }
    #endregion

    public void RoomReady()
    {

    }
}
