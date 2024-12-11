using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SnareFleaStateAI : MonsterAI
{
    [Header("Componenets")]
    public Rigidbody rigidbody;     //SnareFlea의 rigidbody
    public NavMeshAgent navMeshAgent;  //몬스터의 NavMesh
    public SnareFleaHealth health;


    //상태 패턴
    private ISnareFleaState currentState;

    public bool hasSeenPlayer;  //플레이어를 보았음 이 클래스 밖에서 변경됨.
    public Transform player;    //플레이어의 Transform 저장함.
    public Transform playersHead;   //플레이어의 머리 Transform
    
    [Header("Stick")]
    public float bindDistance = 1f;   //Falling시 플레이어에 달라붙는 거리
    public float groundBindDistance = 1.5f; //땅에서 플레이어에 달라붙는 거리
    
    [Header("Boxcast")]
    public Vector3 boxSize = new Vector3(2f, 2f, 1f);
    public Vector3 castDirection = Vector3.down;
    public float castDistance = 50f;
    public int layerMaskWithoutSelf;

    [Header("Run")]
    public bool isAttacked = false; //공격 받음 여부. 공격 받으면 도망간다.
    public bool hasDestination = false;     //navMeshAgent가 목적지를 설정했는지?
    public float runDistance = 8f;
    
    [Header("IsGround")]
    public float checkDistance = 1f;
    public LayerMask groundLayer;

    [Header("Attack")]
    public float lastAttackTime;
    public float attackCooltime = 2f;
    public DamageMessage damageMessage;

    //JumpToCeilling
    public float jumpForce = 100f;

    //Dead


    public SnareFleaStateAI()
    {
        currentState = new IdleState(this);
    }

    private void Start()
    {
        //rigidbody = GetComponent<Rigidbody>();
        //health = GetComponent<SnareFleaHealth>();
        //navMeshAgent = GetComponent<NavMeshAgent>();

        openDoorDelay = 4f;

        damageMessage = new DamageMessage();
        damageMessage.damage = 10;
        damageMessage.damager = gameObject;

        layerMaskWithoutSelf = ~(1 << LayerMask.NameToLayer("Monster"));
    }

    public override void OnStartServer()
    {
        enabled = true;
        navMeshAgent.enabled = true;
        MonsterReference.Instance.AddMonsterToList(gameObject);
        
        //상태 패턴
        currentState.Enter();

        Test_BehaviourTree.Instance.nodeStatus.text = $"{currentState.State}";

        //스폰후 점프
        JumpToCeling();
    }

    public void Update()
    {
        currentState.Update();
    }

    public void ChangeState(ISnareFleaState newState)
    {
        currentState.Exit();

        currentState = newState;

        //Test_BehaviourTree.Instance.nodeStatus.text = $"{currentState.State}";

        currentState.Enter();
    }

    public bool IsGround()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, checkDistance, groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void JumpToCeling()
    {
        Debug.Log($"{gameObject.name} is 천장으로 점핑");

        navMeshAgent.enabled = false;
        rigidbody.useGravity = false;
        rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}

public interface ISnareFleaState
{
    string State { get; }
    void Enter();   //상태 시작시 호출
    void Update();  //매 프레임 호출
    void Exit();    //상태 종료시 호출
}

public class IdleState : ISnareFleaState
{
    private SnareFleaStateAI monster;

    public IdleState(SnareFleaStateAI monster)
    {
        this.monster = monster;
    }

    public string State => "Idle";

    public void Enter()
    {
        Debug.Log($"{monster.name} is Entering Idle State");
    }

    public void Update()
    {
        //죽음 체크
        if (monster.health.IsDead)
        {
            monster.ChangeState(new DeadState(monster));
            return;
        }

        //플레이어를 발견하면 Falling으로
        if (HasSeenPlayer(out Transform player))
        {
            monster.player = player;
            monster.playersHead = player.GetChild(2);
            monster.ChangeState(new FallingState(monster));
        }
    }

    public void Exit()
    {
        Debug.Log($"{monster.name} is Exiting Idle State");
    }

    private bool HasSeenPlayer(out Transform player)
    {
        // 디버깅을 위한 박스 시각화
        Debug.DrawRay(monster.transform.position, monster.castDirection * monster.castDistance, Color.red);

        RaycastHit hit;
        if (Physics.BoxCast(monster.transform.position, monster.boxSize / 2, monster.castDirection, 
            out hit, Quaternion.identity, monster.castDistance, monster.layerMaskWithoutSelf))
        {
            int hitLayer = hit.collider.gameObject.layer;
            if (hitLayer == LayerMask.NameToLayer("Player"))
            {
                player = hit.transform;
                return true;
            }
        }
        player = null;
        return false;
    }
}

public class FallingState : ISnareFleaState
{
    private SnareFleaStateAI monster;

    public FallingState(SnareFleaStateAI monster)
    {
        this.monster = monster;
    }

    public string State => "Falling";

    public void Enter()
    {
        Debug.Log($"{monster.name} is Entering Falling State");

        monster.rigidbody.useGravity = true;

    }

    public void Update()
    {
        //죽음 체크
        if (monster.health.IsDead)
        {
            monster.ChangeState(new DeadState(monster));
            return;
        }

        //플레이어에 닿으면 StickState로
        if (Vector2.Distance(monster.transform.position, monster.playersHead.position) <= monster.bindDistance)
        {
            monster.transform.position = monster.playersHead.position;
            monster.rigidbody.useGravity = false;
            monster.ChangeState(new StickState(monster));
            return;
        }

        //땅에 닿으면 TracingState로
        if (monster.IsGround())
        {
            monster.ChangeState(new TracingState(monster));
        }
    }

    public void Exit()
    {
        Debug.Log($"{monster.name} is Exiting Falling State");
    }
}

public class StickState : ISnareFleaState
{
    private SnareFleaStateAI monster;
    private LivingEntity playerHealth;

    public StickState(SnareFleaStateAI monster)
    {
        this.monster = monster;
    }

    public string State => "Stick";

    public void Enter()
    {
        Debug.Log($"{monster.name} is Entering Stick State");
        playerHealth = monster.player.GetComponent<LivingEntity>();

        monster.rigidbody.useGravity = false;
        monster.navMeshAgent.enabled = false;
    }

    public void Update()
    {
        //죽음 체크
        if (monster.health.IsDead)
        {
            monster.ChangeState(new DeadState(monster));
            return;
        }

        monster.transform.position = monster.playersHead.position;

        if(Time.time - monster.lastAttackTime >= monster.attackCooltime)
        {
            //공격 당하거나, 플레이어 죽었으면 RunawayState로
            if (monster.isAttacked || playerHealth.IsDead)
            {
                monster.rigidbody.useGravity = true;
                monster.ChangeState(new RunawayState(monster));
                return;
            }

            //아니면 데미지 적용.
            else
            {
                playerHealth.ApplyDamage(monster.damageMessage);
                monster.lastAttackTime = Time.time;
            }
        }
    }

    public void Exit()
    {
        Debug.Log($"{monster.name} is Exiting Stick State");
    }
}

public class RunawayState : ISnareFleaState
{
    private SnareFleaStateAI monster;

    public RunawayState(SnareFleaStateAI monster)
    {
        this.monster = monster;
    }

    public string State => "Runaway";

    public void Enter()
    {
        Debug.Log($"{monster.name} is Entering Runaway State");
        monster.navMeshAgent.enabled = true;
    }

    public void Update()
    {
        //죽음 체크
        if (monster.health.IsDead)
        {
            monster.ChangeState(new DeadState(monster));
            return;
        }

        //플레이어로 부터 먼곳으로 목적지를 설정하기.
        if (!monster.hasDestination)
        {
            Vector3 newPos = RandomNavMeshMovement.RandomNavSphere(monster.transform.position, monster.runDistance, -1);
            monster.navMeshAgent.SetDestination(newPos);
            monster.hasDestination = true;
        }

        //목적지에 도착했으면 점프하기
        if(Vector3.Distance(monster.navMeshAgent.destination, monster.transform.position) < 0.5f)
        {
            monster.hasDestination = false;
            monster.JumpToCeling();
            
            //점프 후 대기 상태로.
            monster.ChangeState(new IdleState(monster));
        }
    }

    public void Exit()
    {
        Debug.Log($"{monster.name} is Exiting Runaway State");
    }
}

public class TracingState : ISnareFleaState
{
    private SnareFleaStateAI monster;
    private LivingEntity playerHealth;

    public TracingState(SnareFleaStateAI monster)
    {
        this.monster = monster;
    }

    public string State => "Tracing";
    public void Enter()
    {
        Debug.Log($"{monster.name} is Entering Tracing State");
        monster.rigidbody.useGravity = true;
        monster.navMeshAgent.enabled = true;

        if(monster.player != null)
        {
            playerHealth = monster.player.GetComponent<LivingEntity>();
        }
        else
        {
            Debug.LogWarning("monster.player is null");
        }
    }

    public void Update()
    {
        //죽음 체크
        if (monster.health.IsDead)
        {
            monster.ChangeState(new DeadState(monster));
            return;
        }

        //플레이어가 죽거나, 공격당하면 Runaway State로
        if (monster.isAttacked || monster.player != null || playerHealth.IsDead)
        {
            if (monster.isAttacked)
            {
                Test_BehaviourTree.Instance.nodeStatus.text = $"monster.isAttacked: {monster.isAttacked}";
            }
            else if(monster.player != null)
            {
                Test_BehaviourTree.Instance.nodeStatus.text = $"monster.player != null: {monster.player != null}";
            }
            else if (playerHealth.IsDead)
            {
                Test_BehaviourTree.Instance.nodeStatus.text = $"playerHealth.IsDead: {playerHealth.IsDead}";
            }
            monster.ChangeState(new RunawayState(monster));
            return;
        }
        //아니라면 다가가기
        else
        {
            if (!monster.hasDestination)
            {
                monster.navMeshAgent.enabled = true;
                monster.navMeshAgent.SetDestination(monster.player.position);
                monster.hasDestination = true;
            }
            
            if(Vector3.Distance(monster.transform.position, monster.player.position) < monster.groundBindDistance)
            {
                monster.ChangeState(new StickState(monster));
            }
        }
    }

    public void Exit()
    {
        Debug.Log($"{monster.name} is Exiting Tracing State");
    }
}

public class DeadState : ISnareFleaState
{
    private MonsterAI monster;
    private NavMeshAgent navMeshAgent;

    public DeadState(MonsterAI monster)
    {
        this.monster = monster;
        navMeshAgent = monster.GetComponent<NavMeshAgent>();
    }

    public string State => "Dead";

    public void Enter()
    {
        Debug.Log($"{monster.name} is Entering Dead State");
        navMeshAgent.enabled = false;
    }

    public void Update()
    {

    }

    public void Exit()
    {
        Debug.Log($"{monster.name} is Exiting Dead State");
    }
}