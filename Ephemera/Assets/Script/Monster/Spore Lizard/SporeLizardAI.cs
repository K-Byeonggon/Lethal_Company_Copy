using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SporeLizardAI : MonsterAI
{
    private Node topNode;
    [SerializeField] public NavMeshAgent navMeshAgent;

    public Transform bewareOf;
    [SerializeField] Transform head;
    [SerializeField] Transform pivot;

    //Attack
    public bool isWillingToAttack = false;
    public Transform target;
    [SerializeField] float attackDistance = 1f;
    [SerializeField] float attackCooltime = 1f;
    private float lastAttackTime;
    private bool isCooltime => Time.time - lastAttackTime < attackCooltime;
    private DamageMessage damageMessage;

    //Wander
    [SerializeField] float defaultSpeed = 3.5f;
    [SerializeField] float defaultAccel = 8f;
    [SerializeField] float defaultAngle = 120f;
    [SerializeField] float wanderRadius = 10f;

    //Threaten
    [SerializeField] float threatDuration;
    private float threatTime;
    [SerializeField] bool isThreatening = false;
    [SerializeField] float threatDistance = 3.5f;

    //Spore
    public GameObject sporeParticle;
    [SerializeField] float sporePercentage = 0.7f;
    [SerializeField] bool haveSpore = true;


    //Run&Hide
    [SerializeField] float runRadius = 20f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float runAccel = 16f;
    [SerializeField] float runAngle = 240f;

    //isCorner
    [SerializeField] int cornerRayCount = 8;
    [SerializeField] float cornerDetectionRadius = 5f;
    private Vector3[] rayDirections;

    public override void OnStartServer()
    {
        enabled = true;
        navMeshAgent.enabled = true;
        MonsterReference.Instance.AddMonsterToList(gameObject);
        
        openDoorDelay = 1f;
        rayDirections = new Vector3[cornerRayCount];
        ConstructBehaviorTree();

        damageMessage = new DamageMessage() { damage = 20, damager = gameObject };
    }
    void Update()
    {
        if (isServer)
        {
            topNode.Evaluate();
        }

        Test_BehaviourTree.Instance.nodeStatus.text = $"Current Node: {currentNodeName}";
    }

    private void ConstructBehaviorTree()
    {
        //공격 시퀀스
        ActionNode checkAttackWill = new ActionNode(CheckAttackWill);
        ActionNode trackTarget = new ActionNode(TrackTarget);
        ActionNode attackTarget = new ActionNode(AttackTarget);

        //배회 시퀀스
        ActionNode setDestination = new ActionNode(SetDestination);
        ActionNode wander = new ActionNode(Wander);

        //위협 노드
        ActionNode threaten = new ActionNode(Threaten);

        //도망 시퀀스
        ActionNode readyToRun = new ActionNode(ReadyToRun);
        ActionNode run = new ActionNode(Run);

        //시퀀스 노드들
        SequenceNode attackSequence = new SequenceNode(new List<Node> { checkAttackWill, trackTarget, attackTarget });
        SequenceNode wanderSequence = new SequenceNode(new List<Node> { setDestination, wander });
        SequenceNode runSequence = new SequenceNode(new List<Node> { readyToRun, run });

        //셀렉터 노드(탑 노드)
        topNode = new SelectorNode(new List<Node> { attackSequence, wanderSequence, threaten, runSequence });
    }

    //[공격 시퀀스] 공격 의지 검사
    private Node.State CheckAttackWill()
    {
        currentNodeName = "CheckAttackWill";

        //공격 의지가 활성화 되면
        if (isWillingToAttack && target != null)
        {
            Debug.Log($"[공격 시퀀스] {transform.name}가 공격 의지가 있음.");
            navMeshAgent.destination = target.position;

            return Node.State.SUCCESS;
        }
        else
        {
            return Node.State.FAILURE;
        }
    }

    //[공격 시퀀스] 추적
    private Node.State TrackTarget()
    {
        currentNodeName = "TrackTarget";

        //추적 대상을 잃거나 추적 대상이 죽으면 실패
        if (target == null || target.GetComponent<LivingEntity>().IsDead)
        {
            bewareOf = null;
            return Node.State.FAILURE;
        }
        else if (Vector3.Distance(transform.position, target.position) <= attackDistance)
        {
            Debug.Log($"[공격 시퀀스] {transform.name}이 타겟에 접근 성공.");

            return Node.State.SUCCESS;
        }
        else
        {
            Debug.Log($"[공격 시퀀스] {transform.name}이 타겟에 접근중.");
            //추적 위치 갱신
            navMeshAgent.SetDestination(target.position);

            return Node.State.RUNNING;
        }
    }

    //[공격 시퀀스] 공격
    private Node.State AttackTarget()
    {
        currentNodeName = "AttackTarget";

        LivingEntity player = target.GetComponent<LivingEntity>();

        if(!isCooltime && player != null && !player.IsDead)
        {
            Debug.Log($"{transform.name}이 플레이어 공격");
            player.ApplyDamage(damageMessage);
            lastAttackTime = Time.time;
        }

        //공격에 성공하든 실패하든 다음 프레임에도 공격 수행
        return Node.State.SUCCESS;
    }

    //[배회 시퀀스] 목적지 설정
    private Node.State SetDestination()
    {
        currentNodeName = "SetDestination";

        Vector3 newPos = RandomNavMeshMovement.RandomNavSphere(transform.position, wanderRadius, -1);

        navMeshAgent.SetDestination(newPos);

        return Node.State.SUCCESS;
    }

    //[배회 시퀀스] 목적지 이동
    private Node.State Wander()
    {
        currentNodeName = "Wander";

        //주시 대상이 있으면 배회를 멈춘다.
        if (bewareOf != null)
        {
            //위협을 시작할 테니 위협 시작시간과 위협 지속시간을 갱신해준다. 
            threatTime = Time.time;
            threatDuration = Random.Range(3f, 5f);
            navMeshAgent.isStopped = true;

            return Node.State.FAILURE;
        }
        else if (Vector3.Distance(head.position, navMeshAgent.destination) <= 1f)
        {

            return Node.State.SUCCESS;
        }
        else 
        {
            return Node.State.RUNNING;
        }
    }

    //[위협 노드]
    private Node.State Threaten()
    {
        currentNodeName = "Threaten";

        //플레이어가 근처에서 벗어나면 위협에 성공한다.
        if(bewareOf == null || Vector3.Distance(transform.position, bewareOf.position) > threatDistance)
        {
            currentNodeName = "Threaten.SUCCESS";
            navMeshAgent.isStopped = false;

            return Node.State.SUCCESS;
        }
        //플레이어가 근처에 있으면서 위협 지속시간이 지나지 않았으면 위협(쳐다보기)을 지속한다.
        else if (Time.time - threatTime < threatDuration)
        {
            currentNodeName = "Threaten.RUNNING";
            Vector3 lookPosition = new Vector3(bewareOf.position.x, bewareOf.position.y, bewareOf.position.z);
            pivot.LookAt(lookPosition);

            return Node.State.RUNNING;
        }
        //위협에 실패하면 다음 셀렉터인 도망&숨기를 수행한다.
        else
        {
            currentNodeName = "Threaten.FAILURE";
            navMeshAgent.isStopped = false;

            return Node.State.FAILURE;
        }
    }

    private void ExplodeSpore()
    {
        if (Random.value <= sporePercentage && haveSpore)
        {
            haveSpore = false;
            OnServerInstantiateParticle();
        }
    }
    [Server]
    public void OnServerInstantiateParticle()
    {
        OnClientInstantiateParticle();
    }
    [ClientRpc]
    public void OnClientInstantiateParticle()
    {
        Instantiate(sporeParticle, transform.position, Quaternion.identity);
    }

    //[도망 시퀀스] 도망 준비
    private Node.State ReadyToRun()
    {
        //코너에 몰려있었다면 바로 실패하고 다음 프레임에 공격 시퀀스를 수행한다.
        if (IsInCorner())
        {
            navMeshAgent.speed = defaultSpeed;
            navMeshAgent.acceleration = defaultAccel;
            navMeshAgent.angularSpeed = defaultAngle;

            isWillingToAttack = true;
            target = bewareOf;

            return Node.State.FAILURE;
        }

        //목적지 설정과 Navmesh 속도조절
        currentNodeName = "ReadyToRun";

        navMeshAgent.speed = runSpeed;
        navMeshAgent.acceleration = runAccel;
        navMeshAgent.angularSpeed = runAngle;
        Vector3 newPos = RandomNavMeshMovement.NavAwayFromPlayer(transform.position, bewareOf.position, runRadius);
        navMeshAgent.SetDestination(newPos);

        //도망가기 전에 포자를 발사한다.
        ExplodeSpore();

        return Node.State.SUCCESS;
    }

    //[도망 시퀀스] 도망
    private Node.State Run()
    {
        currentNodeName = "Run";

        if (Vector3.Distance(head.transform.position, navMeshAgent.destination) <= 1f)
        {
            navMeshAgent.speed = defaultSpeed;
            navMeshAgent.acceleration = defaultAccel;
            navMeshAgent.angularSpeed = defaultAngle;

            return Node.State.SUCCESS;
        }
        else
        {
            return Node.State.RUNNING;
        }
    }

    bool IsInCorner()
    {
        int blockedRayCount = 0;
        for (int i = 0; i < cornerRayCount; i++)
        {
            float angle = i * (360f / cornerRayCount);
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            rayDirections[i] = direction;

            NavMeshHit hit;
            if (NavMesh.Raycast(transform.position, transform.position + direction * cornerDetectionRadius, out hit, NavMesh.AllAreas))
            {
                blockedRayCount++;
            }
        }
        return blockedRayCount > cornerRayCount / 2;
    }

    void OnDrawGizmos()
    {
        if (rayDirections == null) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < rayDirections.Length; i++)
        {
            Gizmos.DrawRay(transform.position, rayDirections[i] * cornerDetectionRadius);
        }
    }
}
