using DunGen;
using Mirror;
using Mirror.Examples.CCU;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.ResourceManagement.AsyncOperations;


public class GameManager : NetworkBehaviour
{
    #region Field
    public static GameManager Instance;
    //최대 마감일
    private const int MAX_DEADLINE = 3;
    
    RuntimeDungeon _runtimeDungeon;
    NavMeshGenerator _navMeshGenerator;
    
    Coroutine _timeCoroutine;

    public ShipController shipController;
    public PlayerController localPlayerController;

    [SerializeField] private SpaceSystem spaceSystem;
    
    [SerializeField] private Transform MonsterParent;
    [SerializeField] private Transform ItemParent;
    
    

    /// <summary>
    /// 판매 배율
    /// </summary>
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
    [SyncVar(hook = nameof(OnClientSetCurrentMoney))] private int currentMoney;
    //목표 금액
    [SyncVar(hook = nameof(OnClientSetTargetMoney))] private int targetMoney;
    //남은 마감일
    [SyncVar(hook = nameof(OnClientSetDeadLine))] private int currentDeadline;
    //현재 시간
    [SyncVar(hook = nameof(OnClientDisplayTime))] private int currentTime;
    //선택 행성
    [SyncVar] private Planet selectPlanet;
    //착륙여부
    [SyncVar] public bool IsLand = false;
    #endregion
    #region Property
    public int CurrentMoney
    {
        get => currentMoney;
        set => currentMoney = value;
    }
    public int TargetMoney
    {
        get => targetMoney;
        set => targetMoney = value;
    }
    public int CurrentDeadline
    {
        get => currentDeadline;
        set => currentDeadline = value;
    }
    public int CurrentTime
    {
        get => currentTime;
        set => currentTime = value;
    }
    #endregion

    #region Action
    private event Action TotalRevenueDisplay;
    private event Action<string> CurrentMoneyDisplay;
    private event Action<string> DeadLineDisplay;
    private event Action<string> TargetMoneyDisplay;
    private event Action PlayerStateDisplay;
    private event Action<int> TimeDisplay;
    #endregion
    #region Function
    ///<summary>
    ///terrain활성화
    ///</summary>
    private void SetActivatePlanetTerrain(int index, bool isActive)
    {
        TerrainController.Instance.SetActivePlanetTerrain((Planet)index, isActive);
    }
    private void CreateRoom(int seed)
    {
        //임시 주석
        _runtimeDungeon = Instantiate(ResourceManager.Instance.GetPrefab("DungeonGenerator")).GetComponent<RuntimeDungeon>();
        _runtimeDungeon.Generator.ShouldRandomizeSeed = false;
        _runtimeDungeon.Generator.Seed = seed;
        _runtimeDungeon.Generate();
    }
    private void OnDestoryRoom()
    {
        if(_runtimeDungeon != null)
            Destroy(_runtimeDungeon.gameObject);
        RoomReference.Instance.ClearRoom();
    }
    private void GenerationObject()
    {
        if (isServer)
        {
            _navMeshGenerator = Instantiate(ResourceManager.Instance.GetPrefab("NavMeshBake")).GetComponent<NavMeshGenerator>();
            _navMeshGenerator.BakeNavMesh();
        }
    }
    public void OnPlayerLoadedInit(NetworkConnectionToClient conn)
    {
        localPlayerController?.SetActivateLocalPlayer(false);
        PlayerReference.Instance.localPlayer.controller.PlayerRespawn();
        CameraReference.Instance.SetActiveVirtualCamera(VirtualCameraType.SpaceShipMiniature);
        OnPlayerSceneLoaded(conn);
    }
    
    
    #endregion
    #region MonoBehaviour Function
    private void Awake()
    {
        Instance = this;
    }
    #endregion
    
    #region NetworkBehaviour Function
    /*public override void OnStartServer()
    {
        //모든 플레이어가 준비되었을 시
        OnServerGameReset();
    }*/
    [Command(requiresAuthority = false)]
    private void OnPlayerSceneLoaded(NetworkConnectionToClient conn)
    {
        GameRoomNetworkManager.Instance.playersInGameScene[conn] = true; // 플레이어가 씬에 진입했음을 표시

        // 모든 플레이어가 씬에 진입했는지 확인
        foreach (var entry in GameRoomNetworkManager.Instance.playersInGameScene)
        {
            if (!entry.Value) return; // 하나라도 진입하지 않은 플레이어가 있으면 리턴
        }

        // 모든 플레이어가 씬에 진입한 경우
        OnServerGameReset();
    }
    #endregion
    #region 게임 흐름 제어
    /// <summary>
    /// 게임 리셋(서버에서만 호출)
    /// </summary>
    [Server] public void OnServerGameReset()
    {
        Debug.Log("GameReset");
        currentMoney = 0;
        TargetMoney = 150;
        CurrentDeadline = MAX_DEADLINE;
        
        //캐릭터 제어 비활성화
        OnServerSetActivePlayer(false);
        //클라이언트 초기화
        OnClientGameStartInit();
    }
    #endregion
    #region Server Function 서버에서 실행되는 함수
    /// <summary>
    /// 데드라인 차감 함수
    /// </summary>
    [Server] public void OnServerDayPasses()
    {
        if(CurrentDeadline - 1 < 0)
        {
            //다음 이벤트
            if(CurrentMoney > TargetMoney)
            {
                CurrentMoney = 0;
                TargetMoney = targetMoney * 2;
                CurrentDeadline = MAX_DEADLINE;
            }
            //패배 이벤트
            else
            {
                currentMoney = 0;
                TargetMoney = 150;
                CurrentDeadline = MAX_DEADLINE;
            }
            //캐릭터 제어 비활성화
            OnServerSetActivePlayer(false);
            //클라이언트 초기화
            OnClientGameStartInit();
        }
        else
        {
            CurrentDeadline -= 1;
            OnClientGameStartInit();
        }
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

        CurrentMoney += totalPrice;
        OnClientDisplayTotalRevenue();
    }
    /// <summary>
    /// 성간이동
    /// </summary>
    [Server] public void OnServerStartHyperDrive(int index)
    {
        Debug.Log($"{selectPlanet}, {(Planet)index}");
        if (selectPlanet == (Planet)index)
            return;
        if (spaceSystem.StartWarpDrive(index))
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
        if(shipController == null)
            shipController = FindObjectOfType<ShipController>();
        
        shipController.OnServerChangePosition(TerrainController.Instance.shipStartTransform.position);
        shipController.StartLanding(TerrainController.Instance.GetLandingZone(selectPlanet).position);
        _timeCoroutine = StartCoroutine(IncrementTimeCounter());
        int seed = Environment.TickCount;
        
        Debug.Log("OnServerEnterPlanet");
        OnClientEnterPlanet(seed);

        IsLand = true;
    }
    /// <summary>
    /// 행성 탈출
    /// </summary>
    [Server] public void OnServerEscapePlanet()
    {
        if (shipController == null)
            shipController = FindObjectOfType<ShipController>();

        OnClientSetCharacterController(false);
        OnClientEscapePlanet();
        IsLand = false;

        EscapeSquance();
    }
    /// <summary>
    /// 탈출 시퀸스
    /// </summary>
    [Server] public void EscapeSquance()
    {
        //함선 출발
        shipController.StartEscape(TerrainController.Instance.shipStartTransform.position);
        //게임 시간 비활성화
        if (_timeCoroutine != null)
            StopCoroutine(_timeCoroutine);
        
        StartCoroutine(DestroyAllObjectsAfterDelay());
    }
    /// <summary>
    /// 모든 플레이어 카메라 셋팅
    /// </summary>
    [Server] public void OnServerActiveLocalPlayerCamera()
    {
        OnCLientActiveLocalPlayerCamera();
    }
    /// <summary>
    /// 플레이어 활성화
    /// </summary>
    /// <param name="isActive"></param>
    [Server] public void OnServerSetActivePlayer(bool isActive)
    {
        OnCLientSetActivePlayer(isActive);
    }
    /// <summary>
    /// 문 연결 (서버)
    /// </summary>
    [Server] public void OnServerDoorLinkSequence()
    {
        OnClientDoorLinkSequence();
    }
    /// <summary>
    /// 몬스터 삭제 (서버)
    /// </summary>
    [Server] public void OnServerClearMonster()
    {
        MonsterReference monsterReference = MonsterReference.Instance;
        foreach (GameObject monster in monsterReference.monsterList)
        {
            NetworkServer.Destroy(monster);
        }
        monsterReference.monsterList.Clear();
        OnClientClearMonster();
    }
    /// <summary>
    /// 플레이어 사망 이벤트
    /// </summary>
    [Server] public void PlayerDieEvent()
    {
        bool allDead = true;
        foreach (PlayerHealth player in PlayerReference.Instance.playerDic.Values)
        {
            if (player.dead == false)
            {
                allDead = false;
                break;
            }
        }

        if (allDead)
        {
            Debug.Log("All Player Die");
            OnServerEscapePlanet();
        }
    }
    /// <summary>
    /// 아이템 스폰
    /// </summary>
    [Server] public void SpawnItem()
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
            GameObject item = Instantiate(ResourceManager.Instance.GetPrefab(itemKey[itemIndex]), ItemParent);
            item.transform.position = vec;
            NetworkServer.Spawn(item);
            ItemReference.Instance.AddItemToList(item);
        }
    }
    /// <summary>
    /// 일정 시간이 지난 후에 모든 몬스터와 아이템을 파괴
    /// </summary>
    [Server] private IEnumerator DestroyAllObjectsAfterDelay()
    {
        float delay = 5.0f;
        yield return new WaitForSeconds(delay);

        MonsterReference.Instance.DestroyAll();
        ItemReference.Instance.DestroyAll();
        if (_navMeshGenerator != null)
            Destroy(_navMeshGenerator.gameObject);
        OnClientArriveSpace();
        OnServerDayPasses();

        Debug.Log("VirtualCameraType.SpaceShipMiniature");
        CameraReference.Instance.SetActiveVirtualCamera(VirtualCameraType.SpaceShipMiniature);
    }
    /// <summary>
    /// 몬스터 스폰
    /// </summary>
    [Server] public void SpawnMonster()
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
            GameObject monster = Instantiate(ResourceManager.Instance.GetPrefab(monsterKey[itemIndex]), MonsterParent);
            monster.transform.position = vec;
            NetworkServer.Spawn(monster);
            MonsterReference.Instance.AddMonsterToList(monster);
        }
    }
    #endregion
    #region Command Function 클라이언트에서 호출해서 서버에서 실행하는 함수
    /// <summary>
    /// 오브젝트 파괴
    /// </summary>
    [Command(requiresAuthority = false)]
    public void DestroyObject(NetworkIdentity identity)
    {
        Debug.Log(identity.gameObject.name);
        NetworkServer.Destroy(identity.gameObject);
    }
    #endregion
    #region ClientRpc Function 서버가 원격 프로시저 호출(RPC)로 모든 클라이언트에서 실행되는 함수
    /// <summary>
    /// 캐릭터 제어 활성화
    /// </summary>
    /// <param name="isActive"></param>
    [ClientRpc] public void OnClientSetCharacterController(bool isActive)
    {
        foreach (PlayerHealth player in PlayerReference.Instance.playerDic.Values)
        {
            player.SetActiveCharacterController(isActive);
        }
    }
    [ClientRpc] public void OnClientGameStartInit()
    {
        PlayerReference.Instance.localPlayer.controller.PlayerRespawn();
        CameraReference.Instance.SetActiveVirtualCamera(VirtualCameraType.SpaceShipMiniature);
    }
    [ClientRpc] public void OnClientEnterPlanet(int seed)
    {
        Debug.Log("OnClientEnterPlanet");
        
        CreateRoom(seed);
        GenerationObject();
        
        UIController.Instance.SetActivateUI(null);
        ObjectReference.Instance.GetGameObject("ShipMiniature").SetActive(false);
        spaceSystem.SetActivateSpaceSystem(false);
        SetActivatePlanetTerrain((int)selectPlanet, true);
        CameraReference.Instance.SetActiveVirtualCamera(VirtualCameraType.SpaceShip);
    }
    [ClientRpc] public void OnClientEscapePlanet()
    {
        UIController.Instance.SetActivateUI(null);
        CameraReference.Instance.SetActiveVirtualCamera(VirtualCameraType.SpaceShip);
    }
    [ClientRpc] public void OnClientArriveSpace()
    {
        OnDestoryRoom();
        
        UIController.Instance.SetActivateUI(typeof(UI_Selecter));
        ObjectReference.Instance.GetGameObject("ShipMiniature").SetActive(true);
        spaceSystem.SetActivateSpaceSystem(true);
        SetActivatePlanetTerrain((int)selectPlanet, false);
        CameraReference.Instance.SetActiveVirtualCamera(VirtualCameraType.SpaceShipMiniature);
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
        localPlayerController?.SetActivateLocalPlayer(isActive);
    }
    #endregion
    
    #region ClientRpc Action 서버가 원격 프로시저 호출(RPC)로 모든 클라이언트에서 실행되는 Action
    /// <summary>
    /// 문 연결 (클라이언트)
    /// </summary>
    [ClientRpc] public void OnClientDoorLinkSequence()
    {
        Debug.Log(GameObject.Find("Entrance").transform.GetChild(0).name);
        Debug.Log(TerrainController.Instance.GetFrontDoor(selectPlanet).name);

        LinkDoor extFrontDoor = GameObject.Find("Entrance").transform.GetChild(0).GetComponent<LinkDoor>();
        LinkDoor etrFrontDoor = TerrainController.Instance.GetFrontDoor(selectPlanet).GetComponent<LinkDoor>();

        extFrontDoor.LinkTransform = etrFrontDoor.transform;
        etrFrontDoor.LinkTransform = extFrontDoor.transform;
    }
    /// <summary>
    /// 몬스터 삭제 (클라이언트)
    /// </summary>
    [ClientRpc] public void OnClientClearMonster()
    {
        MonsterReference monsterReference = MonsterReference.Instance;
        monsterReference.monsterList.Clear();
    }
    #endregion
    
    #region Client Hook Function 서버의 SyncVar가 변경되었을 때 호출되는 함수
    /// <summary>
    /// 현재 소지금 변경(모든 클라이언트)
    /// </summary>
    [Client] private void OnClientSetCurrentMoney(int oldValue, int newValue)
    {
        CurrentMoneyDisplay?.Invoke(newValue.ToString());
    }
    /// <summary>
    /// 캐릭터 상태 변경(모든 클라이언트)
    /// </summary>
    [Client] private void OnClientSetPlayerState(int oldValue, int newValue)
    {
        PlayerStateDisplay?.Invoke();
    }
    /// <summary>
    /// 목표 금액 변경(모든 클라이언트)
    /// </summary>
    [Client] private void OnClientSetTargetMoney(int oldValue, int newValue)
    {
        TargetMoneyDisplay?.Invoke(newValue.ToString());
    }
    /// <summary>
    /// 데드라인 변경(모든 클라이언트)
    /// </summary>
    [Client] private void OnClientSetDeadLine(int oldValue, int newValue)
    {
        DeadLineDisplay?.Invoke(newValue.ToString());
    }
    /// <summary>
    /// 시간 UI 출력 (모든 클라이언트)
    /// </summary>
    [Client] private void OnClientDisplayTime(int oldValue, int newValue)
    {
        TimeDisplay?.Invoke(newValue);
    }
    /// <summary>
    /// 총 수익 UI 출력 (모든 클라이언트)
    /// </summary>
    [ClientRpc] private void OnClientDisplayTotalRevenue()
    {
        TotalRevenueDisplay?.Invoke();
    }
    #endregion
    
    #region ActionRegist Action등록
    /// <summary>
    /// 현재 소지금 UI갱신 이벤트 등록
    /// </summary>
    public void RegistCurrentMoneyDisplayAction(Action<string> action = null)
    {
        CurrentMoneyDisplay = action;
    }
    /// <summary>
    /// 목표 금액 UI갱신 이벤트 등록
    /// </summary>
    public void RegistTargetMoneyDisplayAction(Action<string> action = null)
    {
        TargetMoneyDisplay = action;
    }
    /// <summary>
    /// 플레이어 상태 변경 출력 이벤트 등록
    /// </summary>
    public void RegistDeadLineDisplayAction(Action<string> action = null)
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
    public void RegistTimeDisplayAction(Action<int> action = null)
    {
        TimeDisplay = action;
    }
    #endregion
    
    #region IEnumerator
    [Server]
    private IEnumerator IncrementTimeCounter()
    {
        CurrentTime = 0;
        while (true)
        {
            // 1초 대기
            yield return new WaitForSeconds(1.0f);
            // 변수 증가
            CurrentTime++;
            if (CurrentTime >= 960)
            {
                //함선 복귀 이벤트
                yield break;
            }
        }
    }
    #endregion
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