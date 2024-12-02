using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SnareFleaStateAI : MonsterAI
{
    //상태 패턴
    private IMonsterState currentState;

    public bool hasSeenPlayer;  //플레이어를 보았음 이 클래스 밖에서 변경됨.
    public Transform player;    //플레이어의 Transform 저장함.
    public Transform playersHead;   //플레이어의 머리 Transform
    
    public float bindDistance = 1f;   //Falling시 플레이어에 달라붙는 거리
    public float groundBindDistance = 1.5f; //땅에서 플레이어에 달라붙는 거리
    public Rigidbody rigidbody;     //SnareFlea의 rigidbody
    public bool isAttacked = false; //공격 받음 여부. 공격 받으면 도망간다.
    public NavMeshAgent navMeshAgent;  //몬스터의 NavMesh

    //Runaway
    public bool hasDestination = false;     //navMeshAgent가 목적지를 설정했는지?
    public float runDistance = 8f;
    //IsGround
    public float checkDistance = 1f;
    public LayerMask groundLayer;

    //Attack
    public float lastAttackTime;
    public float attackCooltime = 2f;
    public DamageMessage damageMessage;

    //JumpToCeilling
    public float jumpForce = 100f;



    public SnareFleaStateAI()
    {
        currentState = new SnareFleaIdleState(this);
    }

    private void Start()
    {
        damageMessage = new DamageMessage();
        damageMessage.damage = 10;
        damageMessage.damager = gameObject;
    }

    public override void OnStartServer()
    {
        enabled = true;
        navMeshAgent.enabled = true;
        MonsterReference.Instance.AddMonsterToList(gameObject);
        
        //상태 패턴
        currentState.Enter();

        //스폰후 점프
        JumpToCeling();
    }

    public void Update()
    {
        currentState.Update();
    }

    public void ChangeState(IMonsterState newState)
    {
        currentState.Exit();
        currentState = newState;
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

public interface IMonsterState
{
    void Enter();   //상태 시작시 호출
    void Update();  //매 프레임 호출
    void Exit();    //상태 종료시 호출
}

public class SnareFleaIdleState : IMonsterState
{
    private SnareFleaStateAI monster;

    public SnareFleaIdleState(SnareFleaStateAI monster)
    {
        this.monster = monster;
    }

    public void Enter()
    {
        Debug.Log($"{monster.name} is Entering Idle State");
    }

    public void Update()
    {
        //플레이어를 발견하면 Falling으로
        if (monster.hasSeenPlayer)
        {
            monster.ChangeState(new SnareFleaFallingState(monster));
        }
    }

    public void Exit()
    {
        Debug.Log($"{monster.name} is Exiting Idle State");
    }
}

public class SnareFleaFallingState : IMonsterState
{
    private SnareFleaStateAI monster;

    public SnareFleaFallingState(SnareFleaStateAI monster)
    {
        this.monster = monster;
    }

    public void Enter()
    {
        Debug.Log($"{monster.name} is Entering Falling State");
    }

    public void Update()
    {
        //플레이어에 닿으면 StickState로
        if (Vector2.Distance(monster.transform.position, monster.playersHead.position) <= monster.bindDistance)
        {
            monster.transform.position = monster.playersHead.position;
            monster.rigidbody.useGravity = false;
            monster.ChangeState(new SnareFleaStickState(monster));
        }

        //땅에 닿으면 GroundAttackState로
        if (monster.IsGround())
        {
            monster.ChangeState(new SnareFleaGroundAttackState(monster));
        }
    }

    public void Exit()
    {
        Debug.Log($"{monster.name} is Exiting Falling State");
    }
}

public class SnareFleaStickState : IMonsterState
{
    private SnareFleaStateAI monster;
    private LivingEntity playerHealth;

    public SnareFleaStickState(SnareFleaStateAI monster)
    {
        this.monster = monster;
    }

    public void Enter()
    {
        Debug.Log($"{monster.name} is Entering Stick State");
        playerHealth = monster.player.GetComponent<LivingEntity>();

        monster.rigidbody.useGravity = false;
        monster.navMeshAgent.enabled = false;
    }

    public void Update()
    {
        monster.transform.position = monster.playersHead.position;

        if(Time.time - monster.lastAttackTime >= monster.attackCooltime)
        {
            //공격 당하거나, 플레이어 죽었으면 RunawayState로
            if (monster.isAttacked || playerHealth.IsDead)
            {
                monster.rigidbody.useGravity = true;
                monster.ChangeState(new SnareFleaRunawayState(monster));
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

public class SnareFleaRunawayState : IMonsterState
{
    private SnareFleaStateAI monster;

    public SnareFleaRunawayState(SnareFleaStateAI monster)
    {
        this.monster = monster;
    }

    public void Enter()
    {
        Debug.Log($"{monster.name} is Entering Runaway State");
    }

    public void Update()
    {
        //플레이어로 부터 먼곳으로 목적지를 설정하기.
        if (!monster.hasDestination)
        {
            monster.navMeshAgent.enabled = true;
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
            monster.ChangeState(new SnareFleaIdleState(monster));
        }
    }

    public void Exit()
    {
        Debug.Log($"{monster.name} is Exiting Runaway State");
    }
}

public class SnareFleaGroundAttackState : IMonsterState
{
    private SnareFleaStateAI monster;
    private LivingEntity playerHealth;

    public SnareFleaGroundAttackState(SnareFleaStateAI monster)
    {
        this.monster = monster;
    }

    public void Enter()
    {
        Debug.Log($"{monster.name} is Entering GroundAttack State");
        monster.rigidbody.useGravity = true;
        monster.navMeshAgent.enabled = true;

        if(monster.player != null)
        {
            playerHealth = monster.player.GetComponent<LivingEntity>();
        }
    }

    public void Update()
    {
        //플레이어가 죽거나, 공격당하면 Runaway State로
        if(monster.isAttacked || monster.player != null || playerHealth.IsDead)
        {
            monster.ChangeState(new SnareFleaRunawayState(monster));
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
                monster.ChangeState(new SnareFleaStickState(monster));
            }
        }
    }

    public void Exit()
    {
        Debug.Log($"{monster.name} is Exiting GroundAttack State");
    }
}