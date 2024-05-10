using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EphemeraRoomManager : NetworkRoomManager
{
    //서버에서 새로 접속한 클라이언트를 감지했을 때 동작하는 함수
    public override void OnRoomServerConnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerConnect(conn);

        //플레이어 게임오브젝트 생성
        var player = Instantiate(spawnPrefabs[0]);

        //player.transform.position = new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), 0);

        //클라이언트들에게 이 게임오브젝트가 소환되었다는것을 알리고
        //신규 접속한 플레이어의 정보가 담긴 conn을 넘김으로써 player가 conn의 소유임을 알림
        NetworkServer.Spawn(player, conn);
    }
}
