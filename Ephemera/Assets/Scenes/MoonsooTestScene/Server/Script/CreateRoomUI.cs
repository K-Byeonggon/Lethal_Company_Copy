using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRoomUI : MonoBehaviour
{
    public void CreateRoom()
    {
        var manager = EphemeraRoomManager.singleton;

        //방 설정 작업 처리
        //TODO

        //서버를 여는 동시에 클라이언트로써 게임에 참가할 수 있도록 만들어주는 함수
        manager.StartHost();

        //클라이언트로써 게임에 참가할 수 있도록 만들어주는 함수
        //manager.StartClient();
    }
    public void JoinRoom()
    {
        var manager = EphemeraRoomManager.singleton;

        //클라이언트로써 게임에 참가할 수 있도록 만들어주는 함수
        manager.StartClient();
    }
}
