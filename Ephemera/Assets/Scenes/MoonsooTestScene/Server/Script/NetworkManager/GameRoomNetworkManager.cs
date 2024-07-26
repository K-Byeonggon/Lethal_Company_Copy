using Mirror;
using Mirror.Examples.NetworkRoom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoomNetworkManager : NetworkRoomManager
{
    /*[SerializeField]
    GameObject gamePlayerObjectPrefab;
    [SerializeField]
    NetworkRoomPlayer roomPlayerObjectPrefab;
    [SerializeField]
    GameObject spaceSystem;*/

    public static GameRoomNetworkManager Instance => NetworkRoomManager.singleton as GameRoomNetworkManager;

    public override void Start()
    {
        base.Start();
        //playerPrefab = gamePlayerObjectPrefab;
        //roomPlayerPrefab = roomPlayerObjectPrefab;
    }


    /*//새로운 클라이언트가 서버에 연결되었을 때에 서버에서 호출되는 함수
    public override void OnRoomServerConnect(NetworkConnectionToClient conn)//GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("OnRoomServerCreateRoomPlayer");
        GameObject gameobject = Instantiate(ResourceManager.Instance.GetPrefab("RoomPlayer")); //Instantiate(roomPlayerObjectPrefab.gameObject);
        //gameobject의 컴포넌트를 가져와 message로 초기화

        Debug.Log(gameobject.name);
        NetworkServer.AddPlayerForConnection(conn, gameobject);


        GameObject roomCharacter = Instantiate(ResourceManager.Instance.GetPrefab("LobbyScavenger"));
        NetworkServer.Spawn(roomCharacter, conn);
    }
    //클라이언트가 접속했을 때 클라이언트에서 호출되는 함수
    public override void OnRoomClientConnect()
    {
        Debug.Log("OnRoomClientConnect");
    }*/

    //GamePlayer를 생성할 때 호출하는 함수
    /*public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
    {
        GameObject gameobject = Instantiate(ResourceManager.Instance.GetPrefab("Player"), SpawnPoint[SpawnCount].position, SpawnPoint[SpawnCount].rotation);
        //gameobject의 컴포넌트를 가져와 message로 초기화
        SpawnCount++;
        NetworkServer.AddPlayerForConnection(conn, gameobject);
        return gameobject;
    }*/

    /*public override void OnRoomServerSceneChanged(string sceneName)
    {
        Debug.Log(sceneName);
        if (sceneName == "Assets/Scenes/GamePlay.unity")
        {
            GameObject gameManager = Instantiate(ResourceManager.Instance.GetPrefab("GameManager"));
            NetworkServer.Spawn(gameManager);

            //GameObject ship = Instantiate(ResourceManager.Instance.GetPrefab("SpaceShip"));
            //ship.transform.position = new Vector3(3000, 0, 0);
            //NetworkServer.Spawn(ship);

            Transform spawnPointList = ship.GetComponent<ShipController>().spawnPoint;
            foreach (Transform spawnPoint in spawnPointList)
            {
                SpawnPoint.Add(spawnPoint);
            }


            GameObject space = Instantiate(spaceSystem);
            NetworkServer.Spawn(space);

            GameObject terrain = Instantiate(ResourceManager.Instance.GetPrefab("Terrain"));
            NetworkServer.Spawn(terrain);
        }
    }*/
    public override void OnRoomClientExit()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
