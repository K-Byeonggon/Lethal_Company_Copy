using Mirror;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class GameManager : NetworkBehaviour
{
    #region Field
    public static GameManager Instance;

    //��
    [SyncVar]
    private int currentMoney;
    //��ǥ �ݾ�
    [SyncVar]
    private int targetMoney;

    //�ִ� ������
    private const int maxDeadline = 3;

    //���� ������
    [SyncVar]
    private int currentDeadline;

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

    #endregion

    #region Action
    private event Action TotalRevenueDisplay;
    private event Action CurrentMoneyDisplay;
    private event Action DeadLineDisplay;
    private event Action TargetMoneyDisplay;
    private event Action PlayerStateDisplay;
    #endregion

    #region UnityEngine Function
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
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
    [Server]
    public void GameReset()
    {
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
    [Server]
    public void CurrentMoneyChanged(int money)
    {
        currentMoney += money;
        SetCurrentMoney(currentMoney);
    }
    /// <summary>
    /// ��ǥ �ݾ� ����(���������� ȣ��)
    /// </summary>
    [Server]
    public void TargetMoneyChanged(int targetMoney)
    {
        this.targetMoney = targetMoney;
        SetTargetMoney(this.targetMoney);
    }
    /// <summary>
    /// ������� 1 ����
    /// </summary>
    [Server]
    public void DayPasses()
    {
        SetDeadLine(currentDeadline - 1);
    }
    /// <summary>
    /// ������� ����
    /// </summary>
    [Server]
    public void DeadlineReset()
    {
        SetDeadLine(maxDeadline);
    }
    /// <summary>
    /// ������ �Ǹ�
    /// </summary>
    [Server]
    public void SellItem(Item[] items)
    {
        int totalPrice = 0;
        foreach (Item item in items)
        {
            totalPrice += item.itemPrice;
        }

        CurrentMoneyChanged(totalPrice);
        DisplayTotalRevenue();
    }

    [Server]
    public void OnStartHyperDrive(int index)
    {
        spaceSystem.StartWarpDrive(index);
    }

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    /*[Server]
    public void OnServerChangeGameState(GameStateType gameStateType)
    {
        ChangeState(gameStateType);
    }*/
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
    [ClientRpc]
    private void SetCurrentMoney(int money)
    {
        currentMoney = money;
        CurrentMoneyDisplay?.Invoke();
    }
    /// <summary>
    /// ��ǥ �ݾ� ����(��� Ŭ���̾�Ʈ)
    /// </summary>
    [ClientRpc]
    private void SetTargetMoney(int targetMoney)
    {
        this.targetMoney = targetMoney;
        TargetMoneyDisplay?.Invoke();
    }
    /// <summary>
    /// ������� ����(��� Ŭ���̾�Ʈ)
    /// </summary>
    [ClientRpc]
    private void SetDeadLine(int deadLine)
    {
        this.currentDeadline = deadLine;
        DeadLineDisplay?.Invoke();
    }
    /// <summary>
    /// ĳ���� ���� ����(��� Ŭ���̾�Ʈ)
    /// </summary>
    [ClientRpc]
    private void SetPlayerState(int targetMoney)
    {
        this.targetMoney = targetMoney;
        PlayerStateDisplay?.Invoke();
    }
    /// <summary>
    /// �� ���� UI ��� (��� Ŭ���̾�Ʈ)
    /// </summary>
    [ClientRpc]
    private void DisplayTotalRevenue()
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
}
/*
#region GameStateClass ���� ���� Ŭ����
// ���� ���� ResetState - SelectPlanetState - MapLoadState - EntryPlanetState - GameProgressState - ReturnState - GameOverState
/// <summary>
/// ���� ����
/// </summary>
class ResetState : GameState
{
    public ResetState(GameManager manager) : base(manager)
    {
    }

    public override void OnStateEnter()
    {
        gameManager.GameReset();
        gameManager.OnServerChangeGameState(GameStateType.SelectPlanetState);
    }

    public override void OnStateExit()
    {
        
    }
}
/// <summary>
/// �༺ ����
/// </summary>
class SelectPlanetState : GameState
{
    public SelectPlanetState(GameManager manager) : base(manager)
    {
    }

    public override void OnStateEnter()
    {
        //�༺���� UI Ȱ��ȭ
        
    }

    public override void OnStateExit()
    {

    }
}
/// <summary>
/// �� �ε�
/// </summary>
class MapLoadState : GameState
{
    public MapLoadState(GameManager manager) : base(manager)
    {
    }

    public override void OnStateEnter()
    {
        //��� ���� �� �񵿱� �ε�
        //��� �ε� �Ϸ� �� ���� ������ ��ȯ
    }

    public override void OnStateExit()
    {

    }
}
/// <summary>
/// �༺ ����
/// </summary>
class EntryPlanetState : GameState
{
    public EntryPlanetState(GameManager manager) : base(manager)
    {
    }

    public override void OnStateEnter()
    {
        //�༺ ���� ȭ�� ������
        //���� �Ϸ�� GameProgressState�� ����
    }

    public override void OnStateExit()
    {

    }
}
/// <summary>
/// ��ǥ ����
/// </summary>
class GameProgressState: GameState
{
    public GameProgressState(GameManager manager) : base(manager)
    {
    }

    public override void OnStateEnter()
    {
        //��ǥ ����
        //�Լ� ���� ��ư�� ���� �� ReturnState�� ����
        //��� ��� Ȥ�� �ð� �ʰ��� GameOverState�� ����
    }

    public override void OnStateExit()
    {

    }
}
/// <summary>
/// ����
/// </summary>
class ReturnState : GameState
{
    public ReturnState(GameManager manager) : base(manager)
    {
    }

    public override void OnStateEnter()
    {
        //�༺ Ż�� ȭ�� ������
        //Ż�� �Ϸ�� SelectPlanetState�� ����
    }

    public override void OnStateExit()
    {

    }
}
/// <summary>
/// ���� ����
/// </summary>
class GameOverState : GameState
{
    public GameOverState(GameManager manager) : base(manager)
    {
    }

    public override void OnStateEnter()
    {
        //���� �÷��� ���� �ٽ� �ҷ���
    }

    public override void OnStateExit()
    {

    }
}
#endregion
*/