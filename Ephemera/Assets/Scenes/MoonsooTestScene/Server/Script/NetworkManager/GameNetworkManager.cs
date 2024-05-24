using Mirror;
using UnityEngine;

public class GameNetworkManager : NetworkManager
{
    [SerializeField]
    GameObject playerObjectPrefab;

    public static GameNetworkManager Instance => NetworkManager.singleton as GameNetworkManager;

    public override void Start()
    {
        base.Start();
        //spawnPrefabs.Add(roomPlayerPrefab);
        spawnPrefabs.Add(playerObjectPrefab);
    }

    //(�������� ȣ��)ȣ��Ʈ�� ���۵� ���� �����Ͽ� ������ ���۵� �� ȣ��˴ϴ�.
    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<CreateCharacterMessage>(OnCreateCharacter);

        //GameObject prefab = ResourceManager.Instance.GetPrefab("MoonsooTestScene/Server/Prefab/Player.prefab");
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        CreateCharacterMessage characterMessage = new CreateCharacterMessage
        {
            name = numPlayers.ToString(),
        };
        
        NetworkClient.Send(characterMessage);
    }

    //������ ���� �Լ�
    void OnCreateCharacter(NetworkConnectionToClient conn, CreateCharacterMessage message)
    {
        GameObject gameobject = Instantiate(playerObjectPrefab);
        //gameobject�� ������Ʈ�� ������ message�� �ʱ�ȭ

        NetworkServer.AddPlayerForConnection(conn, gameobject);
    }



    //�÷��̾� ������Ʈ ����
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
