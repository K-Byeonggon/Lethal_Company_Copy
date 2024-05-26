using DunGen;
using Mirror;
using Mirror.Examples.CCU;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using static UnityEditor.Progress;


public class GameManager : NetworkBehaviour
{
    #region Field
    public static GameManager Instance;
    //�ִ� ������
    private const int maxDeadline = 3;
    //�Ǹ� ����
    RuntimeDungeon rd;
    Coroutine timeCoroutine;

    public PlayerController localPlayerController;

    public ShipController shipController;
    NavMeshGenerator navMeshGenerator;

    [SyncVar]
    public bool IsLand = false;

    private int SalePriceMagnification
    {
        get
        {
            switch (currentDeadline)
            {
                case 0:
                    return 100;
                case 1:
                    return 75;
                case 2:
                    return 50;
                case 3:
                    return 25;
                default:
                    return 10;
            }
        }
    }
    #endregion
    #region Sync Field
    //������
    [SyncVar] private int currentMoney;
    //��ǥ �ݾ�
    [SyncVar] private int targetMoney;
    //���� ������
    [SyncVar] private int currentDeadline;
    //���� �ð�
    [SyncVar] private int currentTime = 0;
    //���� �༺
    [SyncVar] private Planet selectPlanet;
    #endregion
    #region Property
    public int CurrentMoney => currentMoney;
    #endregion

    #region Function
    public GameTime GetCurrentTime()
    {
        return new GameTime(currentTime);
    }
    ///<summary>
    ///terrainȰ��ȭ
    ///</summary>
    public void SetActivatePlanetTerrain(int index, bool isActive)
    {
        TerrainController.Instance.SetActivePlanetTerrain((Planet)index, isActive);
    }

    public void CreateRoom(int seed)
    {
        //�ӽ� �ּ�
        rd = Instantiate(ResourceManager.Instance.GetPrefab("DungeonGenerator")).GetComponent<RuntimeDungeon>();
        rd.Generator.ShouldRandomizeSeed = false;
        rd.Generator.Seed = seed;
        rd.Generate();


        //���� ù ���� ������ ���� ExitDoor �߰�

        //��ġ�� �� ������Ʈ �߰�
        //rd.Generator.CurrentDungeon.MainPathTiles[0].Entrance.transform.position;

        
        /*GameObject go = Instantiate(ResourceManager.Instance.GetPrefab("duck"));
        go.name = "duck";
        go.transform.position = Vector3.up * 2;
        NetworkServer.Spawn(go);*/
    }
    public void DestoryRoom()
    {
        if(rd != null)
            Destroy(rd.gameObject);
        RoomReference.Instance.ClearRoom();
    }
    
    public void GeneraterObject()
    {
        if (isServer)
        {
            navMeshGenerator = Instantiate(ResourceManager.Instance.GetPrefab("NavMeshBake")).GetComponent<NavMeshGenerator>();
            navMeshGenerator.BakeNavMesh();
            //StartCoroutine(BakeNavMeshCoroutine());
            //NavMeshGenerator navMeshGenerator = rd.GetComponent<NavMeshGenerator>();
            //StartCoroutine(BuildNavMesh(navMeshGenerator.navMeshSurface, new Bounds(Vector3.zero, new Vector3(1000, 1000, 1000))));
        }
    }
    #endregion
    #region Action
    private event Action TotalRevenueDisplay;
    private event Action<string> CurrentMoneyDisplay;
    private event Action<string> DeadLineDisplay;
    private event Action<string> TargetMoneyDisplay;
    private event Action PlayerStateDisplay;
    private event Action TimeDisplay;
    #endregion
    #region MonoBehaviour Function
    private void Awake()
    {
        Instance = this;
    }
    #endregion
    #region NetworkBehaviour Function
    /*public override void OnStartClient()
    {
        //��� �÷��̾ �غ�Ǿ��� ��
        //OnServerChangeGameState(GameStateType.ResetState);
        if (isServer == true)
        {
            OnServerGameReset();
        }
    }*/
    public override void OnStartServer()
    {
        //��� �÷��̾ �غ�Ǿ��� ��
        //OnServerChangeGameState(GameStateType.ResetState);
        if (isServer == true)
        {
            OnServerGameReset();
        }
    }
    #endregion
    #region Server Function �������� ����Ǵ� �Լ�
    /// <summary>
    /// ���� ����(���������� ȣ��)
    /// </summary>
    [Server] public void OnServerGameReset()
    {
        Debug.Log("GameReset");
        //������ ����
        OnClientSetCurrentMoney(0);
        //��ǥ�ݾ� ����
        OnServerTargetMoneyChanged(150);
        //������� ����
        OnServerDeadlineReset();
        //�÷��̾� ����
        OnServerSetActivePlayer(true);
        //ĳ���� ���� ��Ȱ��ȭ
        OnServerSetActiveController(false);
        //���� �ʱ�ȭ
        Invoke("GameReset", 2f);
    }
    [Server]
    public void GameReset()
    {
        OnClientGameStartInit();
    }

    /// <summary>
    /// ���� �ݾ� ����(���������� ȣ��)
    /// </summary>
    [Server] public void OnServerCurrentMoneyChanged(int money)
    {
        currentMoney += money;
        OnClientSetCurrentMoney(currentMoney);
    }
    /// <summary>
    /// ��ǥ �ݾ� ����(���������� ȣ��)
    /// </summary>
    [Server] public void OnServerTargetMoneyChanged(int targetMoney)
    {
        this.targetMoney = targetMoney;
        OnClientSetTargetMoney(this.targetMoney);
    }
    /// <summary>
    /// ������� 1 ����
    /// </summary>
    [Server] public void OnServerDayPasses()
    {
        if(currentDeadline - 1 < 0)
        {
            //���� �̺�Ʈ
            if(currentMoney > targetMoney)
            {
                //������ ����
                //OnClientSetCurrentMoney(0);
                //��ǥ�ݾ� ����
                int newTargetMoney = targetMoney * 2;
                OnServerTargetMoneyChanged(newTargetMoney);
                //������� ����
                OnServerDeadlineReset();
                //���� ����
                OnClientGameStartInit();
                //�÷��̾� ����
                OnServerSetActivePlayer(true);
                //ĳ���� ���� ��Ȱ��ȭ
                OnServerSetActiveController(false);
            }
            //�й� �̺�Ʈ
            else
            {
                OnServerGameReset();
            }
        }
        else
        {
            OnClientSetDeadLine(currentDeadline - 1);
        }
    }
    /// <summary>
    /// ������� ����
    /// </summary>
    [Server] public void OnServerDeadlineReset()
    {
        OnClientSetDeadLine(maxDeadline);
    }
    /// <summary>
    /// ������ �Ǹ�
    /// </summary>
    [Server] public void OnServerSellItem(Item[] items)
    {
        int totalPrice = 0;
        foreach (Item item in items)
        {
            totalPrice += item.ItemPrice;
        }

        OnServerCurrentMoneyChanged(totalPrice);
        OnClientDisplayTotalRevenue();
    }
    /// <summary>
    /// �����̵�
    /// </summary>
    [Server] public void OnServerStartHyperDrive(int index)
    {
        if (SpaceSystem.Instance.StartWarpDrive(index))
        {
            OnClientStartHyperDrive();
            selectPlanet = (Planet)index;
        }
    }
    /// <summary>
    /// �༺ ����
    /// </summary>
    [Server] public void OnServerEnterPlanet()
    {
        if (TerrainController.Instance.GetTerrainCount() <= (int)selectPlanet)
            return;
        //���ּ� �ű��
        if(shipController == null)
            shipController = FindObjectOfType<ShipController>();
        shipController.OnServerChangePosition(TerrainController.Instance.shipStartTransform.position);
        //�Լ� ���
        shipController.StartLanding(TerrainController.Instance.GetLandingZone(selectPlanet).position);
        //���� �ð� Ȱ��ȭ
        timeCoroutine = StartCoroutine(IncrementTimeCounter());
        int seed = OnServerGetRandomSeed();
        OnClientEnterPlanet(seed);

        IsLand = true;
    }
    /// <summary>
    /// �༺ Ż��
    /// </summary>
    [Server]
    public void OnServerEscapePlanet()
    {
        Debug.Log("OnServerEscapePlanet");
        //���ּ� �ű��
        if (shipController == null)
            shipController = FindObjectOfType<ShipController>();

        //ĳ������Ʈ�ѷ� ��Ȱ��ȭ
        OnClientSetCharacterController(false);

        IsLand = false;


        Invoke("EscapeSquance", 3f);
    }
    [Server]
    public void EscapeSquance()
    {
        //�Լ� ���
        shipController.StartEscape(TerrainController.Instance.shipStartTransform.position);
        //���� �ð� Ȱ��ȭ
        if (timeCoroutine != null)
            StopCoroutine(timeCoroutine);

        foreach (var monster in MonsterReference.Instance.monsterList)
        {
            //NetworkIdentity id = monster.GetComponent<NetworkIdentity>();
            NetworkServer.Destroy(monster);
        }
        foreach (var item in ItemReference.Instance.itemList)
        {
            //NetworkIdentity id = item.GetComponent<NetworkIdentity>();
            NetworkServer.Destroy(item);
        }

        StartCoroutine(DestroyAllObjectsAfterDelay());
    }
    [ClientRpc]
    public void OnClientSetCharacterController(bool isActive)
    {
        foreach (PlayerHealth player in PlayerReference.Instance.playerDic.Values)
        {
            player.SetActiveCharacterController(isActive);
        }
    }

    /// <summary>
    /// ���� �õ� ����
    /// </summary>
    [Server] public int OnServerGetRandomSeed()
    {
        return Environment.TickCount;
    }
    /// <summary>
    /// ��� �÷��̾� ī�޶� ����
    /// </summary>
    [Server] public void OnServerActiveLocalPlayerCamera()
    {
        OnCLientActiveLocalPlayerCamera();
    }
    [Server] public void OnServerSetActivePlayer(bool isActive)
    {
        OnCLientSetActivePlayer(isActive);
    }
    [Server] public void OnServerSetActiveController(bool isActive)
    {
        OnClientSetActiveController(isActive);
    }
    #endregion
    #region Command Function Ŭ���̾�Ʈ���� ȣ���ϰ� �������� ����Ǵ� �Լ�
    //�÷��̾� ���� ��ȭ
    /*[Command]
    public void CmdPlayerStateChange(NetworkMessage message)
    {
    //myPlayerStatue.Hp -= message.damage;
    //SetPlayerState(myPlayerStatue);
    }*/
    
    #endregion
    #region ClientRpc Function ������ ���� ���ν��� ȣ��(RPC)�� ��� Ŭ���̾�Ʈ���� ����Ǵ� �Լ�
    [ClientRpc] public void OnClientGameStartInit()
    {
        PlayerReference.Instance.localPlayer.controller.PlayerRespawn();
        CameraReference.Instance.SetActiveVirtualCamera(VirtualCameraType.SpaceShipMiniature);
    }
    [ClientRpc] public void OnClientEnterPlanet(int seed)
    {
        Debug.Log($"seed : {seed}");
        //UI �����
        UIController.Instance.SetActivateUI(null);
        //�̴Ͼ��� Ship �����
        ObjectReference.Instance.GetGameObject("ShipMiniature").SetActive(false);
        //spaceSystem �����
        SpaceSystem.Instance.SetActivateSpaceSystem(false);
        //terrain Ȱ��ȭ�ϰ�
        SetActivatePlanetTerrain((int)selectPlanet, true);
        //�� �����ϰ�
        CreateRoom(seed);
        //����, ������ �����ϰ�
        GeneraterObject();
        //ī�޶� �ű��
        CameraReference.Instance.SetActiveVirtualCamera(VirtualCameraType.SpaceShip);
    }

    [ClientRpc]
    public void OnClientEscapePlanet()
    {
        //�̴Ͼ��� Ship �����ְ�
        ObjectReference.Instance.GetGameObject("ShipMiniature").SetActive(true);
        //spaceSystem �����ְ�
        SpaceSystem.Instance.SetActivateSpaceSystem(true);
        //terrain ��Ȱ��ȭ�ϰ�
        SetActivatePlanetTerrain((int)selectPlanet, false);
        //�� �����ϰ�
        DestoryRoom();
        MonsterReference.Instance.DestroyAll();
        ItemReference.Instance.DestroyAll();


        Debug.Log("VirtualCameraType.SpaceShipMiniature");
        //ī�޶� �ű��
        CameraReference.Instance.SetActiveVirtualCamera(VirtualCameraType.SpaceShipMiniature);
        //UI �����ְ�
        UIController.Instance.SetActivateUI(typeof(UI_Selecter));
    }
    [ClientRpc] public void OnClientStartHyperDrive()
    {
        VolumeController.Instance.StartWarpGlitch();
    }
    [ClientRpc] public void OnCLientActiveLocalPlayerCamera()
    {
        CameraReference.Instance.SetActiveLocalPlayerVirtualCamera();
    }
    [ClientRpc] public void OnCLientSetActivePlayer(bool isActive)
    {
        Debug.Log(localPlayerController);
        localPlayerController?.SetActivateLocalPlayer(isActive);
    }
    [ClientRpc] public void OnClientSetActiveController(bool isActive)
    {
        localPlayerController?.SetActivateLocalController(isActive);
    }
    #endregion
    #region ClientRpc Action ������ ���� ���ν��� ȣ��(RPC)�� ��� Ŭ���̾�Ʈ���� ����Ǵ� Action
    /// <summary>
    /// ���� ������ ����(��� Ŭ���̾�Ʈ)
    /// </summary>
    [ClientRpc] private void OnClientSetCurrentMoney(int money)
    {
        currentMoney = money;
        CurrentMoneyDisplay?.Invoke(currentMoney.ToString());
    }
    /// <summary>
    /// ��ǥ �ݾ� ����(��� Ŭ���̾�Ʈ)
    /// </summary>
    [ClientRpc] private void OnClientSetTargetMoney(int targetMoney)
    {
        this.targetMoney = targetMoney;
        TargetMoneyDisplay?.Invoke(this.targetMoney.ToString());
    }
    /// <summary>
    /// ������� ����(��� Ŭ���̾�Ʈ)
    /// </summary>
    [ClientRpc] private void OnClientSetDeadLine(int deadLine)
    {
        this.currentDeadline = deadLine;
        DeadLineDisplay?.Invoke(currentDeadline.ToString());
    }
    /// <summary>
    /// ĳ���� ���� ����(��� Ŭ���̾�Ʈ)
    /// </summary>
    [ClientRpc] private void OnClientSetPlayerState(int targetMoney)
    {
        this.targetMoney = targetMoney;
        PlayerStateDisplay?.Invoke();
    }
    /// <summary>
    /// �� ���� UI ��� (��� Ŭ���̾�Ʈ)
    /// </summary>
    [ClientRpc] private void OnClientDisplayTotalRevenue()
    {
        TotalRevenueDisplay?.Invoke();
    }
    /// <summary>
    /// �� ���� UI ��� (��� Ŭ���̾�Ʈ)
    /// </summary>
    [ClientRpc] private void OnClientDisplayTime()
    {
        GameTime time = GetCurrentTime();
        //Debug.Log($"{time.hour} : {time.minute.ToString("D2")}");
        TimeDisplay?.Invoke();
    }
    #endregion
    #region ActionRegist Action���
    /// <summary>
    /// ���� ������ UI���� �̺�Ʈ ���
    /// </summary>
    public void RegistCurrentMoneyDisplayAction(Action<string> action = null)
    {
        CurrentMoneyDisplay = action;
    }
    /// <summary>
    /// ��ǥ �ݾ� UI���� �̺�Ʈ ���
    /// </summary>
    public void RegistTargetMoneyDisplayAction(Action<string> action = null)
    {
        TargetMoneyDisplay = action;
    }
    /// <summary>
    /// �÷��̾� ���� ���� ��� �̺�Ʈ ���
    /// </summary>
    public void RegistDeadLineDisplayAction(Action<string> action = null)
    {
        DeadLineDisplay = action;
    }
    /// <summary>
    /// �÷��̾� ���� ���� ��� �̺�Ʈ ���
    /// </summary>
    public void RegistPlayerStateDisplayAction(Action action = null)
    {
        PlayerStateDisplay = action;
    }
    /// <summary>
    /// �� ���� UI ��� �̺�Ʈ ���
    /// </summary>
    public void RegistTotalRevenueDisplayAction(Action action = null)
    {
        PlayerStateDisplay = action;
    }
    /// <summary>
    /// �ð� UI ��� �̺�Ʈ ���
    /// </summary>
    public void RegistTimeDisplayAction(Action action = null)
    {
        TimeDisplay = action;
    }
    #endregion
    #region IEnumerator
    [Server]
    private IEnumerator IncrementTimeCounter()
    {
        currentTime = 0;
        while (true)
        {
            // 1�� ���
            yield return new WaitForSeconds(1.0f);
            // ���� ����
            currentTime++;
            // ������ �� ��� (����׿�)
            //GameTime time = GetCurrentTime();
            OnClientDisplayTime();
            if (currentTime >= 960)
            {
                //�Լ� ���� �̺�Ʈ
                yield break;
            }
        }
    }
    #endregion

    [Server]
    public void SpawnItem()
    {
        int itemSpawnCount = UnityEngine.Random.Range(RoomReference.Instance.RoomCount / 2, RoomReference.Instance.RoomCount);

        List<string> itemKey = new List<string>()
        {
            "asynchronous_motor",
            "Barrel",
            "Camera_on",
            "circular_saw",
            "Crowbar",
            "duck",
            "Laptop",
            "Metal container",
            "Mine",
            "Phone_1",
            "ProFlashlight_Low",
            "Tablet_pc",
        };

        for (int i = 0; i < itemSpawnCount; i++)
        {
            Vector3 vec = RoomReference.Instance.GetRandomPosition();
            int itemIndex = UnityEngine.Random.Range(0, itemKey.Count);
            GameObject item = Instantiate(ResourceManager.Instance.GetPrefab(itemKey[itemIndex]));
            item.transform.position = vec;
            NetworkServer.Spawn(item);
            ItemReference.Instance.AddItemToList(item);
        }
    }
    [Server]
    public void SpawnMonster()
    {
        int monsterSpawnCount = UnityEngine.Random.Range(2, 4);

        List<string> monsterKey = new List<string>()
        {
            "Coilhead",
            "Snare Flea",
            "Spore Lizard",
            "Thumper",
            "Yipee",
        };

        for (int i = 0; i < monsterSpawnCount; i++)
        {
            Vector3 vec = RoomReference.Instance.GetRandomPosition();
            int itemIndex = UnityEngine.Random.Range(0, monsterKey.Count);
            GameObject monster = Instantiate(ResourceManager.Instance.GetPrefab(monsterKey[itemIndex]));
            monster.transform.position = vec;
            NetworkServer.Spawn(monster);
            MonsterReference.Instance.AddMonsterToList(monster);
        }
    }
    [Server]
    public void OnServerDoorLinkSequence()
    {
        OnClientDoorLinkSequence();
    }
    [ClientRpc]
    public void OnClientDoorLinkSequence()
    {
        Debug.Log(GameObject.Find("Entrance").transform.GetChild(0).name);
        Debug.Log(TerrainController.Instance.GetFrontDoor(selectPlanet).name);

        LinkDoor extFrontDoor = GameObject.Find("Entrance").transform.GetChild(0).GetComponent<LinkDoor>();
        LinkDoor etrFrontDoor = TerrainController.Instance.GetFrontDoor(selectPlanet).GetComponent<LinkDoor>();

        extFrontDoor.LinkTransform = etrFrontDoor.transform;
        etrFrontDoor.LinkTransform = extFrontDoor.transform;
    }


    [Server]
    public void OnServerClearMonster()
    {
        MonsterReference monsterReference = MonsterReference.Instance;
        foreach (GameObject monster in monsterReference.monsterList)
        {
            NetworkServer.Destroy(monster);
        }
        monsterReference.monsterList.Clear();
        OnClientClearMonster();
    }
    [ClientRpc]
    public void OnClientClearMonster()
    {
        MonsterReference monsterReference = MonsterReference.Instance;
        monsterReference.monsterList.Clear();
    }
    [Command(requiresAuthority = false)]
    public void DestroyObject(NetworkIdentity identity)
    {
        NetworkServer.Destroy(identity.gameObject);
    }

    /// <summary>
    /// ���� �ð��� ���� �Ŀ� ��� ���Ϳ� �������� �ı�
    /// </summary>
    [Server]
    private IEnumerator DestroyAllObjectsAfterDelay()
    {
        float delay = 5.0f;
        yield return new WaitForSeconds(delay);


        MonsterReference.Instance.DestroyAll();
        ItemReference.Instance.DestroyAll();
        if(navMeshGenerator != null)
            Destroy(navMeshGenerator.gameObject);
        OnClientEscapePlanet();
        OnServerDayPasses();

        Debug.Log("VirtualCameraType.SpaceShipMiniature");
        CameraReference.Instance.SetActiveVirtualCamera(VirtualCameraType.SpaceShipMiniature);
    }
    [Server]
    public void PlayerDieEvent()
    {
        Debug.Log("PlayerDieEvent");
        bool allDead = true;
        foreach (PlayerHealth player in PlayerReference.Instance.playerDic.Values)
        {
            if(player.dead == false)
            {
                allDead = false;
                break;
            }
        }
        if (allDead)
            OnServerEscapePlanet();
    }
    
    /*[ClientRpc]
    public void OnServerDisconnectProcessing(NetworkConnectionToClient conn)
    {
        DisconnectProcessing(conn);
    }
    [ClientRpc]
    public void DisconnectProcessing(NetworkConnectionToClient conn)
    {
        if (PlayerReference.Instance != null)
        {
            PlayerReference.Instance.RemovePlayerToDic(conn.identity.netId);
        }
        if (CameraReference.Instance != null)
        {
            CameraReference.Instance.DeregistPlayerVirtualCamera(conn.identity.netId);
        }
    }*/
}

public struct GameTime
{
    public int hour;
    public int minute;
    public GameTime(int time)
    {
        hour = time / 60 + 8;
        minute = time % 60;
    }
}