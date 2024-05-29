using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPlayer : NetworkRoomPlayer
{
    GameObject roomCharacter;


    #region Command, 클라이언트에서 이를 호출하면 서버에서 이 함수를 실행
    // 룸 캐릭터 정보 초기화 명령을 서버로 보냄
    [Command]
    void CmdRoomInfoInit()
    {
        Debug.Log(connectionToClient.identity.netId);
        // 서버에서 클라이언트의 게임오브젝트에 컴포넌트를 추가하는 RPC 호출
        roomCharacter = Instantiate(ResourceManager.Instance.GetPrefab("LobbyScavenger"));
        NetworkServer.Spawn(roomCharacter, connectionToClient);
        TargetRoomInfoInit(connectionToClient);
    }
    //룸 캐릭터 정보 변경
    [Command]
    public void CmdRoomCharacterInfoChange()
    {

    }
    #endregion

    #region TargetRpc, 서버는 원격 프로시저 호출(RPC)을 사용하여 특정 클라이언트에서 이 기능을 실행
    //룸 캐릭터 정보 초기화 클라이언트
    [TargetRpc]
    public void TargetRoomInfoInit(NetworkConnection conn)
    {
        
    }
    #endregion

    //룸 준비
    public void RoomReady()
    {

    }
}
