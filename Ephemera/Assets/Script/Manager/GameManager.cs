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

    //��
    [SyncVar] private int currentMoney;
    //��ǥ �ݾ�
    [SyncVar] private int targetMoney;

    //�ִ� ������
    private const int maxDeadline = 3;

    //���� ������
    [SyncVar] private int currentDeadline;

    //���� �ð�
    [SyncVar] private int currentTime = 0;

    //���� ���� ����
    //[SyncVar]
    //private GameState currentGameState;

    //private Dictionary<GameStateType, GameState> gameStates = new Dictionary<GameStateType, GameState>();

    //�Ǹ� ����
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

    //�÷��̾� ����
    //List<PlayerStatue> statue;

    //���� �÷��̾� ĳ����
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
    ///terrainȰ��ȭ
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
        //��� �÷��̾ �غ�Ǿ��� ��
        //OnServerChangeGameState(GameStateType.ResetState);
        if(isServer == true)
        {
            GameReset();
        }
    }
    #endregion

    #region Server Function �������� ����Ǵ� �Լ�
    /// <summary>
    /// ���� ����(���������� ȣ��)
    /// </summary>
    [Server] public void GameReset()
    {
        Debug.Log("GameReset");
        //������ ����
        SetCurrentMoney(0);
        //��ǥ�ݾ� ����
        TargetMoneyChanged(150);
        //������� ����
        DeadlineReset();
    }
    /// <summary>
    /// ���� �ݾ� ����(���������� ȣ��)
    /// </summary>
    [Server] public void CurrentMoneyChanged(int money)
    {
        currentMoney += money;
        SetCurrentMoney(currentMoney);
    }
    /// <summary>
    /// ��ǥ �ݾ� ����(���������� ȣ��)
    /// </summary>
    [Server] public void TargetMoneyChanged(int targetMoney)
    {
        this.targetMoney = targetMoney;
        SetTargetMoney(this.targetMoney);
    }
    /// <summary>
    /// ������� 1 ����
    /// </summary>
    [Server] public void DayPasses()
    {
        SetDeadLine(currentDeadline - 1);
    }
    /// <summary>
    /// ������� ����
    /// </summary>
    [Server] public void DeadlineReset()
    {
        SetDeadLine(maxDeadline);
    }
    /// <summary>
    /// ������ �Ǹ�
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
    /// ���� ���� ����
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
        //UI �����
        UIController.SetActivateUI(null);
        //�̴Ͼ��� Ship �����
        Oref.GetGameObject("ShipMiniature").SetActive(false);
        //spaceSystem �����
        spaceSystem.SetActivateSpaceSystem(false);
        //terrain Ȱ��ȭ�ϰ�
        ActivatePlanetTerrain((int)selectPlanet);
        //�� �����ϰ�
        CreateRoom(seed);

        //���ּ� �ű��
        ShipController shipController = FindObjectOfType<ShipController>();
        shipController.transform.position = terrainController.shipStartTransform.position;


        //ī�޶� �ű��
        CameraReference.Instance.SetActiveVirtualCamera(VirtualCameraType.SpaceShip);

        //�Լ� ���
        shipController.StartLanding();

        timeCoroutine = StartCoroutine(IncrementTimeCounter());
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
    /// <summary>
    /// ���� ������ ����(��� Ŭ���̾�Ʈ)
    /// </summary>
    [ClientRpc] private void SetCurrentMoney(int money)
    {
        currentMoney = money;
        CurrentMoneyDisplay?.Invoke();
    }
    /// <summary>
    /// ��ǥ �ݾ� ����(��� Ŭ���̾�Ʈ)
    /// </summary>
    [ClientRpc] private void SetTargetMoney(int targetMoney)
    {
        this.targetMoney = targetMoney;
        TargetMoneyDisplay?.Invoke();
    }
    /// <summary>
    /// ������� ����(��� Ŭ���̾�Ʈ)
    /// </summary>
    [ClientRpc] private void SetDeadLine(int deadLine)
    {
        this.currentDeadline = deadLine;
        DeadLineDisplay?.Invoke();
    }
    /// <summary>
    /// ĳ���� ���� ����(��� Ŭ���̾�Ʈ)
    /// </summary>
    [ClientRpc] private void SetPlayerState(int targetMoney)
    {
        this.targetMoney = targetMoney;
        PlayerStateDisplay?.Invoke();
    }
    /// <summary>
    /// �� ���� UI ��� (��� Ŭ���̾�Ʈ)
    /// </summary>
    [ClientRpc] private void DisplayTotalRevenue()
    {
        TotalRevenueDisplay?.Invoke();
    }
    /// <summary>
    /// ���� ���� ���� (��� Ŭ���̾�Ʈ)
    /// </summary>
    /*[ClientRpc]
    private void ChangeState(GameStateType gameStateType)
    {
        currentGameState?.OnStateExit();
        currentGameState = gameStates[gameStateType];
        currentGameState.OnStateEnter();
    }*/
    #endregion

    #region ActionRegist Action���
    /// <summary>
    /// ���� ������ UI���� �̺�Ʈ ���
    /// </summary>
    public void RegistCurrentMoneyDisplayAction(Action action = null)
    {
        CurrentMoneyDisplay = action;
    }
    /// <summary>
    /// ��ǥ �ݾ� UI���� �̺�Ʈ ���
    /// </summary>
    public void RegistTargetMoneyDisplayAction(Action action = null)
    {
        TargetMoneyDisplay = action;
    }
    /// <summary>
    /// �÷��̾� ���� ���� ��� �̺�Ʈ ���
    /// </summary>
    public void RegistDeadLineDisplayAction(Action action = null)
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
    #endregion

    #region IEnumerator
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
            GameTime time = GetCurrentTime();
            Debug.Log($"{time.hour} : {time.minute}");
            if(currentTime >= 960)
            {
                //�Լ� ���� �̺�Ʈ
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