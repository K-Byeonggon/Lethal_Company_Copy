using DunGen;
using Mirror;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem.XR;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;


public class GameManager : NetworkBehaviour
{
    #region Field

    private static GameManager instance;
    public static GameManager Instance
    {
        get 
        { 
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    //돈
    [SyncVar] private int currentMoney;
    //목표 금액
    [SyncVar] private int targetMoney;

    //최대 마감일
    private const int maxDeadline = 3;

    //남은 마감일
    [SyncVar] private int currentDeadline;

    //현재 시간
    [SyncVar] private int currentTime = 0;

    //현재 게임 상태
    //[SyncVar]
    //private GameState currentGameState;

    //private Dictionary<GameStateType, GameState> gameStates = new Dictionary<GameStateType, GameState>();

    //판매 배율
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

    //플레이어 상태
    //List<PlayerStatue> statue;

    //나의 플레이어 캐릭터
    //PlayerStatue myPlayerStatue;

    //[SerializeField]
    private SpaceSystem spaceSystem;
    public SpaceSystem SpaceSystem { set {  spaceSystem = value; } }

    private TerrainController terrainController;
    public TerrainController TerrainController { set { terrainController = value; } }

    private UIController uiController;
    private UIController UIController
    {
        get
        {
            if(uiController == null)
                uiController = FindObjectOfType<UIController>();
            return uiController;
        }
    }

    private ObjectReference or;
    private ObjectReference Oref
    {
        get
        {
            if(or == null)
                or = FindObjectOfType<ObjectReference>();
            return or;
        }
    }

    [SyncVar] private Planet selectPlanet;

    RuntimeDungeon rd;

    Coroutine timeCoroutine;

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
        terrainController.SetActivePlanetTerrain((Planet)index, true);
    }
    
    public void CreateRoom(int seed)
    {
        rd = Instantiate(ResourceManager.Instance.GetPrefab("DungeonGenerator")).GetComponent<RuntimeDungeon>();
        rd.Generator.Seed = seed;
        rd.Generate();

        //rd.Generator.CurrentDungeon.MainPathTiles[0].Entrance.transform.position;
        //rd.Generator.CurrentDungeon.MainPathTiles.Last().Entrance.transform.position;
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
    #endregion

    #region UnityEngine Function
    public override void OnStartClient()
    {
        //모든 플레이어가 준비되었을 시
        //OnServerChangeGameState(GameStateType.ResetState);
        if(isServer == true)
        {
            GameReset();
        }
    }
    #endregion

    #region Server Function 서버에서 실행되는 함수
    /// <summary>
    /// 게임 리셋(서버에서만 호출)
    /// </summary>
    [Server] public void GameReset()
    {
        Debug.Log("GameReset");
        //소지금 리셋
        SetCurrentMoney(0);
        //목표금액 리셋
        TargetMoneyChanged(150);
        //데드라인 리셋
        DeadlineReset();
    }
    /// <summary>
    /// 소지 금액 변경(서버에서만 호출)
    /// </summary>
    [Server] public void CurrentMoneyChanged(int money)
    {
        currentMoney += money;
        SetCurrentMoney(currentMoney);
    }
    /// <summary>
    /// 목표 금액 변경(서버에서만 호출)
    /// </summary>
    [Server] public void TargetMoneyChanged(int targetMoney)
    {
        this.targetMoney = targetMoney;
        SetTargetMoney(this.targetMoney);
    }
    /// <summary>
    /// 데드라인 1 차감
    /// </summary>
    [Server] public void DayPasses()
    {
        SetDeadLine(currentDeadline - 1);
    }
    /// <summary>
    /// 데드라인 리셋
    /// </summary>
    [Server] public void DeadlineReset()
    {
        SetDeadLine(maxDeadline);
    }
    /// <summary>
    /// 아이템 판매
    /// </summary>
    [Server] public void SellItem(Item[] items)
    {
        int totalPrice = 0;
        foreach (Item item in items)
        {
            totalPrice += item.itemPrice;
        }

        CurrentMoneyChanged(totalPrice);
        DisplayTotalRevenue();
    }
    [Server] public void OnStartHyperDrive(int index)
    {
        spaceSystem.StartWarpDrive(index);
        selectPlanet = (Planet)index;
    }

    /// <summary>
    /// 게임 상태 변경
    /// </summary>
    /*[Server]
    public void OnServerChangeGameState(GameStateType gameStateType)
    {
        ChangeState(gameStateType);
    }*/
    [Server] public void StartGame()
    {
        if (terrainController.GetTerrainCount() <= (int)selectPlanet)
            return;
        LandingGame(GetRandomSeed());
    }
    [Server] public int GetRandomSeed()
    {
        return Environment.TickCount;
    }
    [ClientRpc] public void LandingGame(int seed)
    {
        //UI 숨기고
        UIController.SetActivateUI(null);
        //미니어쳐 Ship 숨기고
        Oref.GetGameObject("ShipMiniature").SetActive(false);
        //spaceSystem 숨기고
        spaceSystem.SetActivateSpaceSystem(false);
        //terrain 활성화하고
        ActivatePlanetTerrain((int)selectPlanet);
        //방 생성하고
        CreateRoom(seed);

        //우주선 옮기고
        ShipController shipController = FindObjectOfType<ShipController>();
        shipController.transform.position = terrainController.shipStartTransform.position;


        //카메라 옮기고
        CameraReference.Instance.SetActiveVirtualCamera(VirtualCameraType.SpaceShip);

        //함선 출발
        shipController.StartLanding();

        timeCoroutine = StartCoroutine(IncrementTimeCounter());
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
    /// <summary>
    /// 현재 소지금 변경(모든 클라이언트)
    /// </summary>
    [ClientRpc] private void SetCurrentMoney(int money)
    {
        currentMoney = money;
        CurrentMoneyDisplay?.Invoke();
    }
    /// <summary>
    /// 목표 금액 변경(모든 클라이언트)
    /// </summary>
    [ClientRpc] private void SetTargetMoney(int targetMoney)
    {
        this.targetMoney = targetMoney;
        TargetMoneyDisplay?.Invoke();
    }
    /// <summary>
    /// 데드라인 변경(모든 클라이언트)
    /// </summary>
    [ClientRpc] private void SetDeadLine(int deadLine)
    {
        this.currentDeadline = deadLine;
        DeadLineDisplay?.Invoke();
    }
    /// <summary>
    /// 캐릭터 상태 변경(모든 클라이언트)
    /// </summary>
    [ClientRpc] private void SetPlayerState(int targetMoney)
    {
        this.targetMoney = targetMoney;
        PlayerStateDisplay?.Invoke();
    }
    /// <summary>
    /// 총 수익 UI 출력 (모든 클라이언트)
    /// </summary>
    [ClientRpc] private void DisplayTotalRevenue()
    {
        TotalRevenueDisplay?.Invoke();
    }
    /// <summary>
    /// 게임 상태 변경 (모든 클라이언트)
    /// </summary>
    /*[ClientRpc]
    private void ChangeState(GameStateType gameStateType)
    {
        currentGameState?.OnStateExit();
        currentGameState = gameStates[gameStateType];
        currentGameState.OnStateEnter();
    }*/
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
    #endregion

    #region IEnumerator
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
            GameTime time = GetCurrentTime();
            Debug.Log($"{time.hour} : {time.minute}");
            if(currentTime >= 960)
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