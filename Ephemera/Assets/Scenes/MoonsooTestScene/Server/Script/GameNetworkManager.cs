using Mirror;
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

    public override void Start()
    {
        spawnPrefabs.Add(playerObjectPrefab);
    }

    //(서버에서 호출)호스트가 시작될 때를 포함하여 서버가 시작될 때 호출됩니다.
    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<CreateCharacterMessage>(OnCreateCharacter);

        //GameObject prefab = ResourceManager.Instance.GetPrefab("MoonsooTestScene/Server/Prefab/Player.prefab");
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        
        // you can send the message here, or wherever else you want
        CreateCharacterMessage characterMessage = new CreateCharacterMessage
        {
            name = playerCount.ToString(),
        };
        
        NetworkClient.Send(characterMessage);
    }

    //프리팹 생성 함수
    void OnCreateCharacter(NetworkConnectionToClient conn, CreateCharacterMessage message)
    {
        GameObject gameobject = Instantiate(playerObjectPrefab);
        //gameobject의 컴포넌트를 가져와 message로 초기화

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
