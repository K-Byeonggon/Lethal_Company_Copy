using Mirror;
using System.Runtime.Serialization;
using UnityEngine;
using static Mirror.Examples.CharacterSelection.NetworkManagerCharacterSelection;

public class GameNetworkManager : NetworkManager
{
    [SerializeField]
    GameObject playerObjectPrefab;
    

    [SerializeField]
    public PrefabReference prefabReference;
    [SerializeField]
    public ObjectReference objectReference;

    int playerCount = 0;

    public static GameNetworkManager Instance => NetworkManager.singleton as GameNetworkManager;

    //(서버에서 호출)호스트가 시작될 때를 포함하여 서버가 시작될 때 호출됩니다.
    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<CreateCharacterMessage>(OnCreateCharacter);
    }
    //(서버에서 호출)클라이언트가 연결될 때 호출
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        GameObject playerObject = Instantiate(playerObjectPrefab);

        // call this to use this gameobject as the primary controller
        NetworkServer.AddPlayerForConnection(conn, playerObject);
        playerCount++;
    }
    //(서버에서 호출)클라이언트가 추가될 때 호출
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = Instantiate(prefabReference.playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);
        //objectReference.camera.SetActive(false);
    }
    //(서버에서 호출)클라이언트가 서버에서 연결 해제할 때 호출
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
    }
    //(클라이언트에서 호출)서버에 연결되면 클라이언트에서 호출됩니다.
    //기본적으로 클라이언트를 준비 상태로 설정하고 플레이어를 추가합니다.
    public override void OnClientConnect()
    {
        base.OnClientConnect();

        // you can send the message here, or wherever else you want
        CreateCharacterMessage characterMessage = new CreateCharacterMessage
        {
            name = playerCount.ToString(),
        };

        NetworkClient.Send(characterMessage);

        //objectReference.camera.SetActive(false);
        //NetworkClient.localPlayer.gameObject.GetComponent<PlayerController>().enabled = true;
    }

    //프리팹 생성 함수
    void OnCreateCharacter(NetworkConnectionToClient conn, CreateCharacterMessage message)
    {
        // playerPrefab is the one assigned in the inspector in Network
        // Manager but you can use different prefabs per race for example
        GameObject gameobject = Instantiate(prefabReference.playerPrefab);

        // call this to use this gameobject as the primary controller
        NetworkServer.AddPlayerForConnection(conn, gameobject);
    }

    //플레이어 오브젝트 변경
    public void ReplacePlayer(NetworkConnectionToClient conn, GameObject newPrefab)
    {
        // Cache a reference to the current player object
        GameObject oldPlayer = conn.identity.gameObject;

        // Instantiate the new player object and broadcast to clients
        // Include true for keepAuthority paramater to prevent ownership change
        NetworkServer.ReplacePlayerForConnection(conn, Instantiate(newPrefab), true);

        // Remove the previous player object that's now been replaced
        // Delay is required to allow replacement to complete.
        Destroy(oldPlayer, 0.1f);
    }
}
