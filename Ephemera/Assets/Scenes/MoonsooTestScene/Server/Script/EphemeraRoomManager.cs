using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.CharacterSelection;
using static Mirror.Examples.CharacterSelection.NetworkManagerCharacterSelection;

public class EphemeraRoomManager : NetworkRoomManager
{
    public bool SpawnAsCharacter = true;

    //서버에서 새로 접속한 클라이언트를 감지했을 때 동작하는 함수
    public override void OnRoomServerConnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerConnect(conn);

        //roomPlayerPrefab;

        //플레이어 게임오브젝트 생성
        var player = Instantiate(spawnPrefabs[0]);

        //클라이언트들에게 이 게임오브젝트가 소환되었다는것을 알리고
        //신규 접속한 플레이어의 정보가 담긴 conn을 넘김으로써 player가 conn의 소유임을 알림
        NetworkServer.Spawn(player, conn);
    }


    private void ReturnToRoomScene(NetworkConnectionToClient conn)
    {
        // increment the index before adding the player, so first player starts at 1
        clientIndex++;

        if (Utils.IsSceneActive(RoomScene))
        {
            allPlayersReady = false;

            //Debug.Log("NetworkRoomManager.OnServerAddPlayer playerPrefab: {roomPlayerPrefab.name}");

            GameObject newRoomGameObject = OnRoomServerCreateRoomPlayer(conn);
            if (newRoomGameObject == null)
                newRoomGameObject = Instantiate(roomPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);

            NetworkServer.AddPlayerForConnection(conn, newRoomGameObject);
        }
        else
        {
            // Late joiners not supported...should've been kicked by OnServerDisconnect
            Debug.Log($"Not in Room scene...disconnecting {conn}");
            conn.Disconnect();
        }
    }

}
