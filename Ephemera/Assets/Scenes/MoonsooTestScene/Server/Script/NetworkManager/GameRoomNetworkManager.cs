using Mirror;
using Mirror.Examples.NetworkRoom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoomNetworkManager : NetworkRoomManager
{
    [SerializeField]
    GameObject gamePlayerObjectPrefab;
    [SerializeField]
    NetworkRoomPlayer roomPlayerObjectPrefab;
    [SerializeField]
    GameObject spaceSystem;

    public GameManager gameManager;

    public static GameRoomNetworkManager Instance => NetworkRoomManager.singleton as GameRoomNetworkManager;
     
    public override void Start()
    {
        base.Start();
        //spawnPrefabs.Add(roomPlayerObjectPrefab.gameObject);
        //spawnPrefabs.Add(gamePlayerObjectPrefab);
        playerPrefab = gamePlayerObjectPrefab;
        roomPlayerPrefab = roomPlayerObjectPrefab;
    }


    //���ο� Ŭ���̾�Ʈ�� ������ ����Ǿ��� ���� �������� ȣ��Ǵ� �Լ�
    public override void OnRoomServerConnect(NetworkConnectionToClient conn)//GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("OnRoomServerCreateRoomPlayer");
        GameObject gameobject = Instantiate(ResourceManager.Instance.GetPrefab("RoomPlayer")); //Instantiate(roomPlayerObjectPrefab.gameObject);
        //gameobject�� ������Ʈ�� ������ message�� �ʱ�ȭ

        Debug.Log(gameobject.name);
        NetworkServer.AddPlayerForConnection(conn, gameobject);


        GameObject roomCharacter = Instantiate(ResourceManager.Instance.GetPrefab("LobbyScavenger"));
        NetworkServer.Spawn(roomCharacter, conn);
    }
    //Ŭ���̾�Ʈ�� �������� �� Ŭ���̾�Ʈ���� ȣ��Ǵ� �Լ�
    public override void OnRoomClientConnect() 
    {
        Debug.Log("OnRoomClientConnect");
        //ResourceManager.Instance.GetPrefab("RoomPlayer");
    }

    //GamePlayer�� ������ �� ȣ���ϴ� �Լ�
    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
    {
        GameObject gameobject = Instantiate(ResourceManager.Instance.GetPrefab("Scavenger")); //Instantiate(gamePlayerObjectPrefab);
        //gameobject�� ������Ʈ�� ������ message�� �ʱ�ȭ

        NetworkServer.AddPlayerForConnection(conn, gameobject);
        return gameobject;
    }

    public override void OnRoomServerSceneChanged(string sceneName)
    {
        Debug.Log(sceneName);
        if(sceneName == "Assets/Scenes/GamePlay.unity")
        {
            Debug.Log("CreateGameManager");
            GameObject gameManager = Instantiate(ResourceManager.Instance.GetPrefab("GameManager"));
            NetworkServer.Spawn(gameManager);
            //NetworkServer.AddConnection(gameManager.GetComponent<NetworkIdentity>().connectionToClient);

            GameObject ship = Instantiate(ResourceManager.Instance.GetPrefab("SpaceShip"));
            ship.transform.position = new Vector3(3000, 0, 0);
            NetworkServer.Spawn(ship);

            GameObject space = Instantiate(spaceSystem);
            NetworkServer.Spawn(space);

            GameObject terrain = Instantiate(ResourceManager.Instance.GetPrefab("Terrain"));
            NetworkServer.Spawn(terrain);
        }
    }


    /*//room�÷��̾� ���� �Լ�
    void OnCreateRoomCharacter(NetworkConnectionToClient conn, CreateRoomCharacterMessage message)
    {
        GameObject gameobject = Instantiate(roomPlayerPrefab.gameObject);
        //gameobject�� ������Ʈ�� ������ message�� �ʱ�ȭ

        NetworkServer.AddPlayerForConnection(conn, gameobject);
    }*/

    /*//(�������� ȣ��)ȣ��Ʈ�� ���۵� ���� �����Ͽ� ������ ���۵� �� ȣ��˴ϴ�.
    public override void OnStartServer()
    {
        base.OnStartServer();

        //NetworkServer.RegisterHandler<CreateCharacterMessage>(OnCreateCharacter);
        //NetworkServer.RegisterHandler<CreateRoomCharacterMessage>(OnCreateRoomCharacter);

        //GameObject prefab = ResourceManager.Instance.GetPrefab("MoonsooTestScene/Server/Prefab/Player.prefab");
    }
    //�����÷��̾� ���� �Լ�
    void OnCreateCharacter(NetworkConnectionToClient conn, CreateCharacterMessage message)
    {
        GameObject gameobject = Instantiate(gamePlayerObjectPrefab);
        //gameobject�� ������Ʈ�� ������ message�� �ʱ�ȭ

        NetworkServer.AddPlayerForConnection(conn, gameobject);
    }
    //room�÷��̾� ���� �Լ�
    void OnCreateRoomCharacter(NetworkConnectionToClient conn, CreateRoomCharacterMessage message)
    {
        GameObject gameobject = Instantiate(roomPlayerPrefab.gameObject);
        //gameobject�� ������Ʈ�� ������ message�� �ʱ�ȭ

        NetworkServer.AddPlayerForConnection(conn, gameobject);
    }*/
}
