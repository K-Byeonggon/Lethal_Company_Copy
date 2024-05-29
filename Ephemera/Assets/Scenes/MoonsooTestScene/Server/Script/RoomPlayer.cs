using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPlayer : NetworkRoomPlayer
{
    GameObject roomCharacter;


    #region Command, Ŭ���̾�Ʈ���� �̸� ȣ���ϸ� �������� �� �Լ��� ����
    // �� ĳ���� ���� �ʱ�ȭ ����� ������ ����
    [Command]
    void CmdRoomInfoInit()
    {
        Debug.Log(connectionToClient.identity.netId);
        // �������� Ŭ���̾�Ʈ�� ���ӿ�����Ʈ�� ������Ʈ�� �߰��ϴ� RPC ȣ��
        roomCharacter = Instantiate(ResourceManager.Instance.GetPrefab("LobbyScavenger"));
        NetworkServer.Spawn(roomCharacter, connectionToClient);
        TargetRoomInfoInit(connectionToClient);
    }
    //�� ĳ���� ���� ����
    [Command]
    public void CmdRoomCharacterInfoChange()
    {

    }
    #endregion

    #region TargetRpc, ������ ���� ���ν��� ȣ��(RPC)�� ����Ͽ� Ư�� Ŭ���̾�Ʈ���� �� ����� ����
    //�� ĳ���� ���� �ʱ�ȭ Ŭ���̾�Ʈ
    [TargetRpc]
    public void TargetRoomInfoInit(NetworkConnection conn)
    {
        
    }
    #endregion

    //�� �غ�
    public void RoomReady()
    {

    }
}
