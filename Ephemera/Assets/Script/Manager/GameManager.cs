using DunGen;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;


public class GameManager : NetworkBehaviour
{
    #region Field
    public static GameManager Instance;
    //최대 마감일
    private const int maxDeadline = 3;
    //판매 배율
    RuntimeDungeon rd;
    Coroutine timeCoroutine;

    public PlayerController localPlayerController;

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
    //소지금
    [SyncVar] private int currentMoney;
    //목표 금액
    [SyncVar] private int targetMoney;
    //남은 마감일
    [SyncVar] private int currentDeadline;
    //현재 시간
    [SyncVar] private int currentTime = 0;
    //선택 행성
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
    ///terrain활성화
    ///</summary>
    public void ActivatePlanetTerrain(int index)
    {
        TerrainController.Instance.SetActivePlanetTerrain((Planet)index, true);
    }

    public void CreateRoom(int seed)
    {
        //임시 주석
        rd = Instantiate(ResourceManager.Instance.GetPrefab("DungeonGenerator")).GetComponent<RuntimeDungeon>();
        rd.Generator.Seed = seed;
        rd.Generate();


        //내부 첫 문과 마지막 문에 ExitDoor 추가

        //위치에 문 오브젝트 추가
        //rd.Generator.CurrentDungeon.MainPathTiles[0].Entrance.transform.position;

        
        /*GameObject go = Instantiate(ResourceManager.Instance.GetPrefab("duck"));
        go.name = "duck";
        go.transform.position = Vector3.up * 2;
        NetworkServer.Spawn(go);*/
    }
    public void GeneraterObject()
    {
        if (isServer)
        {
            NavMeshGenerator navMeshGenerator = Instantiate(ResourceManager.Instance.GetPrefab("NavMeshBake")).GetComponent<NavMeshGenerator>();
            navMeshGenerator.BakeNavMesh();
            //StartCoroutine(BakeNavMeshCoroutine());
            //NavMeshGenerator navMeshGenerator = rd.GetComponent<NavMeshGenerator>();
            //StartCoroutine(BuildNavMesh(navMeshGenerator.navMeshSurface, new Bounds(Vector3.zero, new Vector3(1000, 1000, 1000))));
        }
    }
    public void DestroyRoom()
    {
        Destroy(rd.gameObject);
    }
    #endregion
    #region Action
    private event Action TotalRevenueDisplay;
    private event Action CurrentMoneyDisplay;
    private event Action DeadLineDisplay;
    private event Action TargetMoneyDisplay;
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
    public override void OnStartClient()
    {
        //모든 플레이어가 준비되었을 시
        //OnServerChangeGameState(GameStateType.ResetState);
        if (isServer == true)
        {
            OnServerGameReset();
        }
    }
    #endregion
    #region Server Function 서버에서 실행되는 함수
    /// <summary>
    /// 게임 리셋(서버에서만 호출)
    /// </summary>
    [Server] public void OnServerGameReset()
    {
        Debug.Log("GameReset");
        //소지금 리셋
        OnClientSetCurrentMoney(0);
        //목표금액 리셋
        OnServerTargetMoneyChanged(150);
        //데드라인 리셋
        OnServerDeadlineReset();
        //카메라 리셋
        OnClientGameStartInit();

        OnServerSetActivePlayer(true);
        //캐릭터 제어 비활성화
        OnServerSetActiveController(false);
    }
    /// <summary>
    /// 소지 금액 변경(서버에서만 호출)
    /// </summary>
    [Server] public void OnServerCurrentMoneyChanged(int money)
    {
        currentMoney += money;
        OnClientSetCurrentMoney(currentMoney);
    }
    /// <summary>
    /// 목표 금액 변경(서버에서만 호출)
    /// </summary>
    [Server] public void OnServerTargetMoneyChanged(int targetMoney)
    {
        this.targetMoney = targetMoney;
        OnClientSetTargetMoney(this.targetMoney);
    }
    /// <summary>
    /// 데드라인 1 차감
    /// </summary>
    [Server] public void OnServerDayPasses()
    {
        OnClientSetDeadLine(currentDeadline - 1);
    }
    /// <summary>
    /// 데드라인 리셋
    /// </summary>
    [Server] public void OnServerDeadlineReset()
    {
        OnClientSetDeadLine(maxDeadline);
    }
    /// <summary>
    /// 아이템 판매
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
    /// 성간이동
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
    /// 행성 진입
    /// </summary>
    [Server] public void OnServerEnterPlanet()
    {
        if (TerrainController.Instance.GetTerrainCount() <= (int)selectPlanet)
            return;
        //우주선 옮기고
        ShipController shipController = FindObjectOfType<ShipController>();
        shipController.OnServerChangePosition(TerrainController.Instance.shipStartTransform.position);
        //함선 출발
        shipController.StartLanding(TerrainController.Instance.GetLandingZone(selectPlanet).position);
        //게임 시간 활성화
        timeCoroutine = StartCoroutine(IncrementTimeCounter());

        OnClientEnterPlanet(OnServerGetRandomSeed());
    }
    /// <summary>
    /// 랜덤 시드 생성
    /// </summary>
    [Server] public int OnServerGetRandomSeed()
    {
        return Environment.TickCount;
    }
    /// <summary>
    /// 모든 플레이어 카메라 셋팅
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
    #region Command Function 클라이언트에서 호출하고 서버에서 실행되는 함수
    //플레이어 상태 변화
    /*[Command]
    public void CmdPlayerStateChange(NetworkMessage message)
    {
    //myPlayerStatue.Hp -= message.damage;
    //SetPlayerState(myPlayerStatue);
    }*/
    
    #endregion
    #region ClientRpc Function 서버가 원격 프로시저 호출(RPC)로 모든 클라이언트에서 실행되는 함수
    [ClientRpc] public void OnClientGameStartInit()
    {
        CameraReference.Instance.SetActiveVirtualCamera(VirtualCameraType.SpaceShipMiniature);
    }
    [ClientRpc] public void OnClientEnterPlanet(int seed)
    {
        //UI 숨기고
        UIController.Instance.SetActivateUI(null);
        //미니어쳐 Ship 숨기고
        ObjectReference.Instance.GetGameObject("ShipMiniature").SetActive(false);
        //spaceSystem 숨기고
        SpaceSystem.Instance.SetActivateSpaceSystem(false);
        //terrain 활성화하고
        ActivatePlanetTerrain((int)selectPlanet);
        //방 생성하고
        CreateRoom(seed);
        //몬스터, 아이템 생성하고
        GeneraterObject();
        //카메라 옮기고
        CameraReference.Instance.SetActiveVirtualCamera(VirtualCameraType.SpaceShip);
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
    #region ClientRpc Action 서버가 원격 프로시저 호출(RPC)로 모든 클라이언트에서 실행되는 Action
    /// <summary>
    /// 현재 소지금 변경(모든 클라이언트)
    /// </summary>
    [ClientRpc] private void OnClientSetCurrentMoney(int money)
    {
        currentMoney = money;
        CurrentMoneyDisplay?.Invoke();
    }
    /// <summary>
    /// 목표 금액 변경(모든 클라이언트)
    /// </summary>
    [ClientRpc] private void OnClientSetTargetMoney(int targetMoney)
    {
        this.targetMoney = targetMoney;
        TargetMoneyDisplay?.Invoke();
    }
    /// <summary>
    /// 데드라인 변경(모든 클라이언트)
    /// </summary>
    [ClientRpc] private void OnClientSetDeadLine(int deadLine)
    {
        this.currentDeadline = deadLine;
        DeadLineDisplay?.Invoke();
    }
    /// <summary>
    /// 캐릭터 상태 변경(모든 클라이언트)
    /// </summary>
    [ClientRpc] private void OnClientSetPlayerState(int targetMoney)
    {
        this.targetMoney = targetMoney;
        PlayerStateDisplay?.Invoke();
    }
    /// <summary>
    /// 총 수익 UI 출력 (모든 클라이언트)
    /// </summary>
    [ClientRpc] private void OnClientDisplayTotalRevenue()
    {
        TotalRevenueDisplay?.Invoke();
    }
    /// <summary>
    /// 총 수익 UI 출력 (모든 클라이언트)
    /// </summary>
    [ClientRpc] private void OnClientDisplayTime()
    {
        GameTime time = GetCurrentTime();
        //Debug.Log($"{time.hour} : {time.minute.ToString("D2")}");
        TimeDisplay?.Invoke();
    }
    #endregion
    #region ActionRegist Action등록
    /// <summary>
    /// 현재 소지금 UI갱신 이벤트 등록
    /// </summary>
    public void RegistCurrentMoneyDisplayAction(Action action = null)
    {
        CurrentMoneyDisplay = action;
    }
    /// <summary>
    /// 목표 금액 UI갱신 이벤트 등록
    /// </summary>
    public void RegistTargetMoneyDisplayAction(Action action = null)
    {
        TargetMoneyDisplay = action;
    }
    /// <summary>
    /// 플레이어 상태 변경 출력 이벤트 등록
    /// </summary>
    public void RegistDeadLineDisplayAction(Action action = null)
    {
        DeadLineDisplay = action;
    }
    /// <summary>
    /// 플레이어 상태 변경 출력 이벤트 등록
    /// </summary>
    public void RegistPlayerStateDisplayAction(Action action = null)
    {
        PlayerStateDisplay = action;
    }
    /// <summary>
    /// 총 수익 UI 출력 이벤트 등록
    /// </summary>
    public void RegistTotalRevenueDisplayAction(Action action = null)
    {
        PlayerStateDisplay = action;
    }
    /// <summary>
    /// 시간 UI 출력 이벤트 등록
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
            // 1초 대기
            yield return new WaitForSeconds(1.0f);
            // 변수 증가
            currentTime++;
            // 증가한 값 출력 (디버그용)
            //GameTime time = GetCurrentTime();
            OnClientDisplayTime();
            if (currentTime >= 960)
            {
                //함선 복귀 이벤트
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