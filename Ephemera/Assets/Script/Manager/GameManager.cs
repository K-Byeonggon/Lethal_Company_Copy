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
    public static GameManager Instance;
    //�ִ� ������
    private const int maxDeadline = 3;
    //�Ǹ� ����
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
    public void ActivatePlanetTerrain(int index)
    {
        TerrainController.Instance.SetActivePlanetTerrain((Planet)index, true);
    }

    public void CreateRoom(int seed)
    {
        rd = Instantiate(ResourceManager.Instance.GetPrefab("DungeonGenerator")).GetComponent<RuntimeDungeon>();
        rd.Generator.Seed = seed;
        rd.Generate();

        /*//���� ù ���� ������ ���� ExitDoor �߰�
        ExitDoor extFrontDoor = rd.Generator.CurrentDungeon.MainPathTiles[0].Entrance.gameObject.AddComponent<ExitDoor>();
        ExitDoor extBackDoor = rd.Generator.CurrentDungeon.MainPathTiles.Last().Entrance.gameObject.AddComponent<ExitDoor>();
        //�ܺ� ù ���� ������ ���� EntryDoor �߰�
        EntryDoor etrFrontDoor = TerrainController.Instance.GetFrontDoor(selectPlanet).AddComponent<EntryDoor>();
        EntryDoor etrBackDoor = TerrainController.Instance.GetBackDoor(selectPlanet).AddComponent<EntryDoor>();

        extFrontDoor.entryPosition = etrFrontDoor.transform;
        etrFrontDoor.exitPosition = extFrontDoor.transform;
        extBackDoor.entryPosition = etrBackDoor.transform;
        etrBackDoor.exitPosition = extBackDoor.transform;*/
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
        //��� �÷��̾ �غ�Ǿ��� ��
        //OnServerChangeGameState(GameStateType.ResetState);
        if(isServer == true)
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
        //ī�޶� ����
        OnClientGameStartInit();
        OnServerSetActivePlayer(true);
        //ĳ���� ���� ��Ȱ��ȭ
        OnServerSetActiveController(false);
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
        OnClientSetDeadLine(currentDeadline - 1);
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
        ShipController shipController = FindObjectOfType<ShipController>();
        shipController.OnServerChangePosition(TerrainController.Instance.shipStartTransform.position);
        //�Լ� ���
        shipController.StartLanding(TerrainController.Instance.GetLandingZone(selectPlanet).position);
        //���� �ð� Ȱ��ȭ
        timeCoroutine = StartCoroutine(IncrementTimeCounter());

        OnClientEnterPlanet(OnServerGetRandomSeed());
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
        CameraReference.Instance.SetActiveVirtualCamera(VirtualCameraType.SpaceShipMiniature);
    }
    [ClientRpc] public void OnClientEnterPlanet(int seed)
    {
        //UI �����
        UIController.Instance.SetActivateUI(null);
        //�̴Ͼ��� Ship �����
        ObjectReference.Instance.GetGameObject("ShipMiniature").SetActive(false);
        //spaceSystem �����
        SpaceSystem.Instance.SetActivateSpaceSystem(false);
        //terrain Ȱ��ȭ�ϰ�
        ActivatePlanetTerrain((int)selectPlanet);
        //�� �����ϰ�
        CreateRoom(seed);
        //ī�޶� �ű��
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
    #region ClientRpc Action ������ ���� ���ν��� ȣ��(RPC)�� ��� Ŭ���̾�Ʈ���� ����Ǵ� Action
    /// <summary>
    /// ���� ������ ����(��� Ŭ���̾�Ʈ)
    /// </summary>
    [ClientRpc] private void OnClientSetCurrentMoney(int money)
    {
        currentMoney = money;
        CurrentMoneyDisplay?.Invoke();
    }
    /// <summary>
    /// ��ǥ �ݾ� ����(��� Ŭ���̾�Ʈ)
    /// </summary>
    [ClientRpc] private void OnClientSetTargetMoney(int targetMoney)
    {
        this.targetMoney = targetMoney;
        TargetMoneyDisplay?.Invoke();
    }
    /// <summary>
    /// ������� ����(��� Ŭ���̾�Ʈ)
    /// </summary>
    [ClientRpc] private void OnClientSetDeadLine(int deadLine)
    {
        this.currentDeadline = deadLine;
        DeadLineDisplay?.Invoke();
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