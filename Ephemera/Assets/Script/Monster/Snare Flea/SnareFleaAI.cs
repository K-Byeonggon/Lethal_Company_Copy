using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SnareFleaAI : MonsterAI
{

    private Node topNode;
    [SerializeField] public UnityEngine.AI.NavMeshAgent navMeshAgent;
    [SerializeField] SnareFleaHealth snareHealth;
    private DamageMessage damageMessage;
    [SerializeField] float attackCooltime = 2f;
    private float lastAttackTime;
    public Transform player;

    public bool sawPlayer = false;
    public bool atCeiling = false;
    public bool isGrounded;
    public float checkDistance = 1.0f; // 땅을 체크할 최대 거리
    public LayerMask groundLayer; // 땅 레이어 설정
    public LayerMask ceilingLayer; // 천장 레이어 설정
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] float bindDistance = 1f;
    [SerializeField] float attackDistance = 1.5f;
    [SerializeField] float runDistance = 8f;
    [SerializeField] float jumpForce = 100f;
    public bool isAttacked = false;
    public bool isRunning = false;
    public bool jumped = false;
    float jumpedTime = 0;

    //boxCast
    public Vector3 boxSize = new Vector3(2f, 2f, 1f);
    public Vector3 castDirection = Vector3.down;
    public float castDistance = 50f;
    int layerMaskWithoutSelf;

    void Start()
    {
        openDoorDelay = 4f;
        ConstructBehaviorTree();

        damageMessage = new DamageMessage();
        damageMessage.damage = 10;
        damageMessage.damager = gameObject;
        layerMaskWithoutSelf = ~(1 << LayerMask.NameToLayer("Monster"));

    }
    public override void OnStartServer()
    {
        Debug.LogError("OnStartServer");
        //enabled = true;
        navMeshAgent.enabled = true;
        MonsterReference.Instance.AddMonsterToList(gameObject);

        //첫 스폰후 점프
        JumpToCeiling();
    }
    void Update()
    {
        if (isServer)
        {
            CheckGround();
            //CheckCeiling();
            topNode.Evaluate();
        }
    }

    private void JumpToCeiling()
    {
        Debug.Log("점프");
        jumped = true;
        navMeshAgent.enabled = false;
        rigidbody.useGravity = false;
        rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        jumpedTime = Time.time;
    }

    private void ConstructBehaviorTree()
    {
        //죽음 시퀀스
        ActionNode dead = new ActionNode(Dead);

        //공격 시퀀스
        ActionNode detect = new ActionNode(Detect);
        ActionNode falling = new ActionNode(Falling);
        ActionNode attackPlayer = new ActionNode(AttackPlayer);

        //땅공격 시퀀스
        ActionNode moveToPlayer = new ActionNode(MoveToPlayer);
        //ActionNode attackPlayer

        ActionNode runFromPlayer = new ActionNode(RunFromPlayer);
        ActionNode toCeiling = new ActionNode(ToCeiling);

        SequenceNode attackSequence = new SequenceNode(new List<Node> { detect, falling, attackPlayer });
        SequenceNode groundAttackSequence = new SequenceNode(new List<Node> { moveToPlayer, attackPlayer });
        SequenceNode runSequence = new SequenceNode(new List<Node> { runFromPlayer, toCeiling });

        topNode = new SelectorNode(new List<Node> { dead, attackSequence, groundAttackSequence, runSequence });
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

    /* 행동트리에서 빼고, 첫 스폰에만 동작하도록 따로뺌
    private Node.State Spawned()
    {
        if(!sawPlayer && isGrounded && !jumped)
        {
            Debug.Log("점프");
            jumped = true;
            navMeshAgent.enabled = false;
            rigidbody.useGravity = false;
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpedTime = Time.time;
        }
        return Node.State.FAILURE;
    }
    */

    //[탐색 시퀀스] 플레이어 탐지해서 떨어지기
    private Node.State Detect()
    {
        //그냥 따로 뭐 트리거 체크 하는 거보다 여기서 레이캐스트로 플레이어 찾는게 더 좋을 듯?
        RaycastHit hit;
        if(Physics.BoxCast(transform.position, boxSize / 2, castDirection, out hit, Quaternion.identity, castDistance, layerMaskWithoutSelf))
        {
            int hitLayer = hit.collider.gameObject.layer;
            if (hitLayer == LayerMask.NameToLayer("Player"))
            {
                sawPlayer = true;
                player = hit.transform;
                Debug.Log($"올무벌레: 플레이어 {player.name} 감지");
            }
        }
        // 디버깅을 위한 박스 시각화
        Debug.DrawRay(transform.position, castDirection * castDistance, Color.red);


        if (sawPlayer)
        {
            Debug.Log("올무벌레: [공격시퀀스]Detect 성공");
            rigidbody.useGravity = true;
            return Node.State.SUCCESS;
        }
        else
        {
            return Node.State.RUNNING;
        }
    }


    //[공격 시퀀스] 플레이어가 가까우면 달라붙기. 플레이어가 가진 모든 아이템을 떨어뜨려야한다.
    private Node.State Falling()
    {
        //player.GetChild(0): 플레이어의 머리부분의 빈 오브젝트
        if(Vector3.Distance(transform.position, player.GetChild(2).position) <= bindDistance)
        {
            transform.position = player.GetChild(2).position;
            rigidbody.useGravity = false;


            Debug.Log($"올무벌레: [공격시퀀스]Falling 성공 - {player.name}에 달라붙음");
            return Node.State.SUCCESS;
        }
        else
        {
            if (isGrounded)
            {
                Debug.Log("올무벌레: [공격시퀀스]Falling 실패 - 다음 셀렉터로");
                return Node.State.FAILURE;
            }
            else
            {
                return Node.State.RUNNING;
            }
        }
    }

    //[공격 시퀀스, 지상 공격 시퀀스] 달라붙어 있으면, 일정시간마다 데미지를 준다. 플레이어 죽으면 떨어지기.
    private Node.State AttackPlayer()
    {
        //계속 플레이어의 머리에 위치함.(달라붙은 상태)
        transform.position = player.GetChild(2).position;
        
        //공격 받으면 다음 시퀀스로(공격 -> 지상공격 -> 도망, 지상공격->도망)
        if (isAttacked) { rigidbody.useGravity = true; return Node.State.FAILURE; }

        if (Vector3.Distance(transform.position, player.GetChild(2).position) <= bindDistance)
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

            }
            
            //공격은 RUNNING과 FAILURE만 있음. 달라붙어있는 동안 다른 행동을 할 필요없이 계속 공격할 수 있기 때문.
            return Node.State.RUNNING;
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
            //달라붙기 까지 하고 바로 다음 공격 노드로
            Debug.Log($"올무벌레: [지상 공격시퀀스]플레이어로 이동 성공 - {player.name}에 달라붙음");
            navMeshAgent.enabled = false;
            transform.position = player.GetChild(2).position;
            rigidbody.useGravity = false;

            return Node.State.SUCCESS;
        }
        else return Node.State.RUNNING;
    }

    /* MoneToPlayer에 합쳐졌다.
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
    */

    //[도망 시퀀스] 플레이어로 부터 멀어지고, 점프
    private Node.State RunFromPlayer()
    {
        Debug.Log("도망 시퀀스");

        //첫 목적지 설정
        if (!isRunning)
        {
            navMeshAgent.enabled = true;
            Vector3 newPos = RandomNavMeshMovement.RandomNavSphere(transform.position, runDistance, -1);
            Debug.Log("목적지 설정: " + newPos);
            navMeshAgent.SetDestination(newPos);
            isRunning = true;
        }

        if (Vector3.Distance(navMeshAgent.destination, transform.position) < 0.5f)
        {
            Debug.Log("올무벌레:[도망시퀀스] RunFromPlayer 성공");

            //점프
            JumpToCeiling();

            return Node.State.SUCCESS;
        }
        else return Node.State.RUNNING;
        
        
        //else { return Node.State.FAILURE; }
    }

    //[도망 시퀀스] 점프후 천장에 붙기까지
    private Node.State ToCeiling()
    {
        if (atCeiling)
        {
            Debug.Log("올무벌레:[도망시퀀스] 천장에 붙기 성공.");
            return Node.State.SUCCESS;
        }
        else
        {
            return Node.State.RUNNING;
        }
    }

    /* 천장에 붙는 것은 그냥 따로 적용됨
    //[도망 시퀀스] 천장에 붙기. 기타 bool 초기화.
    private Node.State HangOn()
    {
        Debug.Log("천장 붙기!!" + atCeiling);
        //Debug.Log(transform.position.y);
        Debug.Log("시간: " + Time.time + " 점프: " + jumpedTime);
        if (atCeiling)
        {
            Debug.Log("천장 고정");
            isAttacked = false;
            rigidbody.useGravity = false;
            sawPlayer = false;
            player = null;
            isRunning = false;
            jumped = false;
            return Node.State.SUCCESS;
        }
        else
        {
            Debug.Log("천장 고정이 안되~");
            return Node.State.RUNNING;
        }
    }
    */

    public void SetDefault()
    {
        isAttacked = false;
        rigidbody.useGravity = false;
        sawPlayer = false;
        player = null;
        isRunning = false;
        jumped = false;
    }

    void CheckGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, checkDistance, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
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

    bool CheckCeiling2()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit, checkDistance, ceilingLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * checkDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * checkDistance);
    }
}
