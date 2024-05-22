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
    [SerializeField] float attackDistance = 1.5f;
    [SerializeField] float runDistance = 8f;
    [SerializeField] float jumpForce = 100f;
    public bool isAttacked = false;
    public bool isRunning = false;
    public bool jumped = false;

    void Start()
    {
        navMeshAgent = transform.GetComponent<NavMeshAgent>();
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

        //탐색 시퀀스
        ActionNode detect = new ActionNode(Detect);

        //공격 시퀀스
        ActionNode bind = new ActionNode(Bind);
        ActionNode attackPlayer = new ActionNode(AttackPlayer);

        //지상 공격 시퀀스
        ActionNode moveToPlayer = new ActionNode(MoveToPlayer);
        ActionNode bindFromGround = new ActionNode(BindFromGround);
        //ActionNode attackPlayer

        //도망 시퀀스
        ActionNode runFromPlayer = new ActionNode(RunFromPlayer);
        ActionNode toCeiling = new ActionNode(ToCeiling);
        ActionNode hangOn = new ActionNode(HangOn);

        SequenceNode attackSequence = new SequenceNode(new List<Node> { bind, attackPlayer });
        SequenceNode groundAttackSequence = new SequenceNode(new List<Node> { moveToPlayer, bindFromGround, attackPlayer });
        SequenceNode runSequence = new SequenceNode(new List<Node> { runFromPlayer, toCeiling, hangOn });


        topNode = new SelectorNode(new List<Node> { dead, detect, attackSequence, groundAttackSequence, runSequence });
        
    }

    private Node.State Dead()
    {
        if (snareHealth.IsDead)
        {
            navMeshAgent.enabled = false;
            return Node.State.SUCCESS;
        }
        else return Node.State.FAILURE;
    }

    //[탐색 시퀀스] 플레이어 탐지해서 떨어지기
    private Node.State Detect()
    {
        if (sawPlayer)
        {
            rigidbody.useGravity = true;
            return Node.State.FAILURE;
        }
        else
        {
            return Node.State.SUCCESS;
        }
    }


    //[공격 시퀀스] 플레이어가 가까우면 달라붙기. 플레이어가 가진 모든 아이템을 떨어뜨려야한다.
    private Node.State Bind()
    {
        //player.GetChild(0): 플레이어의 머리부분의 빈 오브젝트
        if(Vector3.Distance(transform.position, player.GetChild(0).position) <= bindDistance)
        {
            Debug.Log("달라붙음");
            transform.position = player.GetChild(0).position;
            rigidbody.useGravity = false;
            return Node.State.SUCCESS;
        }
        else
        {
            if (isGrounded)
            {
                Debug.Log("달라붙기 실패");
                return Node.State.FAILURE;
            }
            else
            {
                Debug.Log("떨어지는 중");
                return Node.State.RUNNING;
            }
        }
    }

    //[공격 시퀀스, 지상 공격 시퀀스] 달라붙어 있으면, 일정시간마다 데미지를 준다. 플레이어 죽으면 떨어지기.
    private Node.State AttackPlayer()
    {
        //공격 받으면 다음 시퀀스로(공격 -> 지상공격 -> 도망, 지상공격->도망)
        if (isAttacked) { rigidbody.useGravity = true; return Node.State.FAILURE; }

        if (Vector3.Distance(transform.position, player.GetChild(0).position) <= bindDistance)
        {   
            if (Time.time - lastAttackTime >= attackCooltime)
            {
                Debug.Log("공격한다.");
                LivingEntity playerHealth = player.GetComponent<LivingEntity>();

                //플레이어 죽었으면 다음 시퀀스로
                if (playerHealth.IsDead) { rigidbody.useGravity = true; return Node.State.FAILURE; }

                //아니면 데미지 적용.
                playerHealth.ApplyDamage(damageMessage);
                lastAttackTime = Time.time;

                return Node.State.SUCCESS;
            }
            else return Node.State.RUNNING;
        }
        else { return Node.State.FAILURE; }
    }

    //[지상 공격 시퀀스] 플레이어를 향해 다가옴
    private Node.State MoveToPlayer()
    {


        //공격당하면/플레이어 죽었으면 다음 시퀀스인 도망 시퀀스로
        if (isAttacked || player.GetComponent<LivingEntity>().IsDead) { return Node.State.FAILURE; }

        //내비메쉬 켜준다.
        navMeshAgent.enabled = true;
        navMeshAgent.SetDestination(player.position);


        if (Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            return Node.State.SUCCESS;
        }
        else return Node.State.RUNNING;
    }

    //[지상 공격 시퀀스] 지상에서 플레이어에 달라붙기
    private Node.State BindFromGround()
    {
        //지상 공격은 플레이어 본체와의 거리로 계산
        if (Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            Debug.Log("달라붙음");
            navMeshAgent.enabled = false;
            transform.position = player.GetChild(0).position;
            rigidbody.useGravity = false;
            return Node.State.SUCCESS;
        }
        else
        {
            Debug.Log("달라붙기 실패");
            return Node.State.FAILURE;
        }
    }

    //[도망 시퀀스] 플레이어로 부터 멀어짐
    private Node.State RunFromPlayer()
    {
        Debug.Log("도망 시퀀스");
        if (isGrounded)
        {
            if (!isRunning)
            {
                navMeshAgent.enabled = true;
                Debug.Log("목적지 설정");
                Vector3 newPos = RandomNavMeshMovement.RandomNavSphere(transform.position, runDistance, -1);
                Debug.Log(newPos);
                navMeshAgent.SetDestination(newPos);
                isRunning = true;
            }
            return Node.State.SUCCESS;
        }
        else { return Node.State.FAILURE; }
    }

    //[도망 시퀀스] 도망목적지에 도착했으면 점프.
    private Node.State ToCeiling()
    {
        Debug.Log("천장에 붙는거");
        if (Vector3.Distance(navMeshAgent.destination, transform.position) < 0.5f)
        {
            if (isGrounded && !jumped)
            {
                Debug.Log("점프");
                jumped = true;
                navMeshAgent.enabled = false;
                rigidbody.useGravity = false;
                rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
        return Node.State.SUCCESS;
    }
    
    //[도망 시퀀스] 천장에 붙기. 기타 bool 초기화.
    private Node.State HangOn()
    {
        Debug.Log("여기 오긴 했니?");
        if (atCeiling)
        {
            Debug.Log("천장 고정");
            isAttacked = false;
            rigidbody.useGravity = false;
            sawPlayer = false;
            player = null;
            return Node.State.SUCCESS;
        }
        else
        {
            return Node.State.RUNNING;
        }
    }


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
