using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SnareFleaAI : MonsterAI
{

    private Node topNode;
    public UnityEngine.AI.NavMeshAgent navMeshAgent;
    SnareFleaHealth snareHealth;
    private DamageMessage damageMessage;
    [SerializeField] float attackCooltime = 2f;
    private float lastAttackTime;
    public Transform player;

    public bool sawPlayer = false;
    [SerializeField] bool atCeiling = false;
    [SerializeField] bool isGrounded;
    public float checkDistance = 1.0f; // 땅을 체크할 최대 거리
    public LayerMask groundLayer; // 땅 레이어 설정
    public LayerMask ceilingLayer; // 천장 레이어 설정
    private Rigidbody rigidbody;
    [SerializeField] float bindDistance = 1f;
    public bool isAttacked = false;
    public bool isRunning = false;

    void Start()
    {
        //navMeshAgent = transform.parent.GetComponent<NavMeshAgent>();
        ConstructBehaviorTree();

        snareHealth = GetComponent<SnareFleaHealth>();
        rigidbody = GetComponent<Rigidbody>();
        damageMessage = new DamageMessage();
        damageMessage.damage = 10;
        damageMessage.damager = gameObject;
    }

    void Update()
    {
        CheckGround();
        CheckCeiling();
        topNode.Evaluate();
    }

    private void ConstructBehaviorTree()
    {
        //죽음 시퀀스의 children Node
        ActionNode dead = new ActionNode(Dead);

        ActionNode detect = new ActionNode(Detect);

        //공격 시퀀스
        ActionNode bind = new ActionNode(Bind);
        ActionNode attackPlayer = new ActionNode(AttackPlayer);

        //지상 도망 시퀀스
        ActionNode runFromPlayer = new ActionNode(RunFromPlayer);
        ActionNode toCeiling = new ActionNode(ToCeiling);

        SequenceNode attackSequence = new SequenceNode(new List<Node> { bind, attackPlayer });
        SequenceNode runSequence = new SequenceNode(new List<Node> { runFromPlayer, toCeiling });
        topNode = new SelectorNode(new List<Node> { dead, detect, attackSequence, runSequence });
    }

    private Node.State Dead()
    {
        if (snareHealth.IsDead)
        {
            navMeshAgent.SetDestination(transform.position);
            return Node.State.SUCCESS;
        }
        else return Node.State.FAILURE;
    }

    //[탐색 시퀀스] 플레이어 탐지하기
    private Node.State Detect()
    {
        if (sawPlayer)
        {
            rigidbody.useGravity = true;
            return Node.State.FAILURE;
        }
        else return Node.State.SUCCESS;
    }

    //[공격 시퀀스] 플레이어가 가까우면 달라붙기. 플레이어가 가진 모든 아이템을 떨어뜨려야한다.
    private Node.State Bind()
    {
        if(Vector3.Distance(transform.position, player.position) <= bindDistance)
        {
            transform.position = player.GetChild(0).position;
            rigidbody.useGravity = false;
            return Node.State.SUCCESS;
        }
        else
        {
            return Node.State.FAILURE;
        }
    }

    //[공격 시퀀스] 달라붙어 있으면, 일정시간마다 데미지를 준다.
    private Node.State AttackPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= bindDistance)
        {
            if (Time.time - lastAttackTime >= attackCooltime)
            {
                Debug.Log("공격한다.");
                LivingEntity playerHealth = player.GetComponent<LivingEntity>();

                //플레이어 죽었으면 배회 시퀀스로
                if (playerHealth.IsDead) { rigidbody.useGravity = true; return Node.State.FAILURE; }

                //아니면 데미지 적용.
                playerHealth.ApplyDamage(damageMessage);
                lastAttackTime = Time.time;

                return Node.State.SUCCESS;
            }
        }
        return Node.State.FAILURE;
    }

    //[지상 도망 시퀀스] 플레이어로 부터 멀어짐
    private Node.State RunFromPlayer()
    {
        if (isAttacked)
        {
            if (!isRunning)
            {
                Vector3 newPos = RandomNavMeshMovement.RandomAwayFromPlayer(transform.position, 2f, -1, player.position);
                navMeshAgent.SetDestination(transform.position);
                isRunning = true;
            }
            return Node.State.SUCCESS;
        }
        else { return Node.State.FAILURE; }
    }

    //[지상 도망 시퀀스] 도망목적지에 도착했으면 천장에 붙음
    
    private Node.State ToCeiling()
    {
        if (isGrounded)
        {
            rigidbody.AddForce(Vector3.up * 100f);
        }
        return Node.State.SUCCESS;
    }

    //[지상 공격 시퀀스] 플레이어를 향해 다가옴
    /*
    private Node.State MoveToPlayer()
    {
        if(Vector2.Distance(transform.position, player.position) <= bindDistance)
        {
            navMeshAgent.SetDestination(player.position);

        }
    }*/


    //[천장 시퀀스] 땅에 있고, 목표 플레이어가 없으면 천장에 붙는다.


    void CheckGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, checkDistance, groundLayer))
        {
            isGrounded = true;
            Debug.Log("Grounded");
        }
        else
        {
            isGrounded = false;
            Debug.Log("Not Grounded");
        }
    }

    void CheckCeiling()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit, checkDistance, ceilingLayer))
        {
            atCeiling = true;
            Debug.Log("atCeiling");
        }
        else
        {
            atCeiling = false;
            Debug.Log("Not at Ceiling");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * checkDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * checkDistance);
    }
}
