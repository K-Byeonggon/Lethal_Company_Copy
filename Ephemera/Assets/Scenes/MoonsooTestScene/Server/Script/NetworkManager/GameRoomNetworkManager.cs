using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoomNetworkManager : NetworkRoomManager
{
    [SerializeField]
    GameObject gamePlayerObjectPrefab;
    [SerializeField]
    NetworkRoomPlayer roomPlayerObjectPrefab;

    public override void Start()
    {
        base.Start();
        spawnPrefabs.Add(roomPlayerObjectPrefab.gameObject);
        spawnPrefabs.Add(gamePlayerObjectPrefab);
        playerPrefab = gamePlayerObjectPrefab;
        roomPlayerPrefab = roomPlayerObjectPrefab;
    }

    //(서버에서 호출)호스트가 시작될 때를 포함하여 서버가 시작될 때 호출됩니다.
    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<CreateCharacterMessage>(OnCreateCharacter);
        NetworkServer.RegisterHandler<CreateRoomCharacterMessage>(OnCreateRoomCharacter);

        //GameObject prefab = ResourceManager.Instance.GetPrefab("MoonsooTestScene/Server/Prefab/Player.prefab");
    }
    public override void OnRoomStartClient() 
    {
        Debug.Log("OnRoomStartClient");

        CreateRoomCharacterMessage characterRoomMessage = new CreateRoomCharacterMessage
        {
            name = numPlayers.ToString(),
        };

        NetworkClient.Send(characterRoomMessage);
    }
    public override void OnRoomClientEnter()
    {
        base.OnRoomClientEnter();
    }
    public override void OnClientConnect()
    {
        Debug.Log("OnClientConnect");
        base.OnClientConnect();

        CreateCharacterMessage characterMessage = new CreateCharacterMessage
        {
            name = numPlayers.ToString(),
        };

        NetworkClient.Send(characterMessage);
    }

    //게임플레이어 생성 함수
    void OnCreateCharacter(NetworkConnectionToClient conn, CreateCharacterMessage message)
    {
        GameObject gameobject = Instantiate(gamePlayerObjectPrefab);
        //gameobject의 컴포넌트를 가져와 message로 초기화

        NetworkServer.AddPlayerForConnection(conn, gameobject);
    }
    //room플레이어 생성 함수
    void OnCreateRoomCharacter(NetworkConnectionToClient conn, CreateRoomCharacterMessage message)
    {
        GameObject gameobject = Instantiate(roomPlayerPrefab.gameObject);
        //gameobject의 컴포넌트를 가져와 message로 초기화

        NetworkServer.AddPlayerForConnection(conn, gameobject);
    }
}
