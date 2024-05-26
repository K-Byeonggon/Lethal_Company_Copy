using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SporeLizardAI : MonsterAI
{
    private Node topNode;
    public UnityEngine.AI.NavMeshAgent navMeshAgent;
    private DamageMessage damageMessage;
    [SerializeField] float attackCooltime = 1f;
    private float lastAttackTime;
    [SerializeField] float threatDuration;
    private float threatTime;
    [SerializeField] bool isThreatening = false;
    [SerializeField] float threatDistance = 3.5f;
    [SerializeField] float runDistance = 2f;
    [SerializeField] float wanderRadius = 10f;
    [SerializeField] float sporePercentage = 0.7f;
    [SerializeField] bool haveSpore = true;
    [SerializeField] float runRadius = 20f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float runAccel = 16f;
    [SerializeField] float runAngle = 240f;
    [SerializeField] float defaultSpeed = 3.5f;
    [SerializeField] float defaultAccel = 8f;
    [SerializeField] float defaultAngle = 120f;
    Vector3 wanderDest;
    public bool setDesti = false;
    public Transform bewareOf;
    [SerializeField] Transform head;
    [SerializeField] Transform pivot;
    public GameObject sporeParticle;
    [SerializeField] int cornerRayCount = 8;
    [SerializeField] float cornerDetectionRadius = 5f;
    [SerializeField] float attackDistance = 1f;
    [SerializeField] bool attackState = false;


    //새로운 버전의 AI
    //유효하지 않은 목적지
    public enum State
    {
        Wander,
        Threaten,
        Explode,
        Run,
        Attack
    }
    public State currentState;
    public bool sawPlayer = false;
    private Vector3[] rayDirections;


    void Start()
    {
        //openDoorDelay = 1f;
        currentState = State.Wander;
        rayDirections = new Vector3[cornerRayCount];
        ConstructBehaviorTree();

        damageMessage = new DamageMessage();
        damageMessage.damage = 20;
        damageMessage.damager = gameObject;
    }

    void Update()
    {
        topNode.Evaluate();
    }

    private void ConstructBehaviorTree()
    {

        //위협 시퀀스의 children Node
        ActionNode threaten = new ActionNode(Threaten);
        ActionNode explodeSpore = new ActionNode(ExplodeSpore);

        //도망
        ActionNode run = new ActionNode(Run);

        //공격 시퀀스의 children Node들
        ActionNode moveToPlayer = new ActionNode(MoveToPlayer);
        ActionNode attackPlayer = new ActionNode(AttackPlayer);

        //배회 시퀀스의 children Node들
        ActionNode wander = new ActionNode(Wander);

        //셀렉터 노드에 들어갈 시퀀스 노드들
        SequenceNode attackSequence = new SequenceNode(new List<Node> { moveToPlayer, attackPlayer });
        topNode = new SelectorNode(new List<Node> { wander, threaten, explodeSpore, run, attackSequence });
    }

    //[배회 시퀀스] 목적지 설정 및 이동
    private Node.State Wander()
    {
        if (currentState != State.Wander) return Node.State.FAILURE;

        if (sawPlayer)
        {
            if (!bewareOf.GetComponent<LivingEntity>().IsDead)
            {
                currentState = State.Threaten;
                setDesti = false;
                return Node.State.FAILURE;
            }
        }

        if (!setDesti)
        {
            Debug.Log("목적지 설정");
            Vector3 newDest = RandomNavMeshMovement.RandomNavSphere(transform.position, wanderRadius, -1);
            setDesti = true;
            navMeshAgent.SetDestination(newDest);
        }

        if (Vector3.Distance(head.position, navMeshAgent.destination) <= 1f)
        {
            Debug.Log("목적지 도착");
            setDesti = false;
            return Node.State.SUCCESS;
        }
        else return Node.State.RUNNING;
    }

    //[위협 시퀀스] 플레이어 응시
    private Node.State Threaten()
    {
        //위협 대기시간 3~5초 동안 위협
        //또는 플레이어 진짜 가까이 다가오면 도망감.
        if (currentState != State.Threaten) return Node.State.FAILURE;

        if (Vector3.Distance(transform.position, bewareOf.position) < threatDistance)
        {
            if (!isThreatening)
            {
                isThreatening = true;
                threatTime = Time.time;
                threatDuration = Random.Range(3f, 5f);
            }

            //위협하기
            if (Time.time - threatTime < threatDuration)
            {
                //rigidbody.velocity = Vector3.zero;
                //rigidbody.isKinematic = true;
                navMeshAgent.isStopped = true;
                Vector3 lookPosition = new Vector3(bewareOf.position.x, 0, bewareOf.position.z);
                pivot.LookAt(lookPosition);
                Debug.Log("위협 중");

                return Node.State.RUNNING;
            }
            else
            {
                //rigidbody.isKinematic = false;
                navMeshAgent.isStopped = false;
                Debug.Log("위협 끝");
                isThreatening = false;
                currentState = State.Explode;
                return Node.State.FAILURE;
            }
        }
        else
        {
            //rigidbody.isKinematic = false;
            navMeshAgent.isStopped = false;
            Debug.Log("위협 도중 종료");
            currentState = State.Wander;
            isThreatening = false;
            return Node.State.FAILURE;
        }
    }

    //[포자 시퀀스] 포자 발사
    private Node.State ExplodeSpore()
    {
        if (currentState != State.Explode) return Node.State.FAILURE;

        if (Random.value <= sporePercentage)
        {
            if (haveSpore)
            {
                Debug.Log("포자 발사");
                haveSpore = false;
                Instantiate(sporeParticle, transform.position, Quaternion.identity);
            }
        }

        //구석이었으면 공격시퀀스로
        if (IsInCorner()) { haveSpore = true; currentState = State.Attack; return Node.State.FAILURE; }
        else { haveSpore = true; currentState = State.Run; return Node.State.FAILURE; }
    }

    //[도망 시퀀스] 도망목적지 설정 및 도망
    private Node.State Run()
    {
        if (currentState != State.Run) return Node.State.FAILURE;

        if (!setDesti)
        {
            Debug.Log("도망 목적지 설정");
            wanderDest = RandomNavMeshMovement.NavAwayFromPlayer(transform.position, bewareOf.position, runRadius);
            navMeshAgent.SetDestination(wanderDest);
            setDesti = true;
            navMeshAgent.speed = runSpeed;
            navMeshAgent.acceleration = runAccel;
            navMeshAgent.angularSpeed = runAngle;
        }

        if (Vector3.Distance(head.transform.position, wanderDest) <= 1f)
        {
            Debug.Log("도망 목적지 도착");
            setDesti = false;
            navMeshAgent.speed = defaultSpeed;
            navMeshAgent.acceleration = defaultAccel;
            navMeshAgent.angularSpeed = defaultAngle;
            bewareOf = null;

            currentState = State.Wander;
            return Node.State.SUCCESS;
        }
        else return Node.State.RUNNING;
    }

    //[공격 시퀀스] 플레이어에게 접근
    private Node.State MoveToPlayer()
    {
        if (currentState != State.Attack) return Node.State.FAILURE;

        float distance = Vector3.Distance(head.transform.position, bewareOf.position);
        Debug.Log("공격하러 이동" + head.transform.position + ", " + bewareOf.position + ", " + Vector3.Distance(head.transform.position, bewareOf.position));
        if (distance > attackDistance)
        {
            Debug.Log("아직 도달하지 못했다.");
            navMeshAgent.SetDestination(bewareOf.position);
            return Node.State.RUNNING;
        }
        else
        {
            Debug.Log("도달했다");
            return Node.State.SUCCESS;
        }
    }

    //[공격 시퀀스] 플레이어를 공격
    private Node.State AttackPlayer()
    {
        Debug.Log("플레이어 공격 노드");
        //Debug.Log(head.position + ", " + bewareOf.position + ", " + Vector3.Distance(head.position, bewareOf.position));

        if (Vector3.Distance(head.transform.position, bewareOf.position) <= attackDistance)
        {
            if (Time.time - lastAttackTime >= attackCooltime)
            {
                Debug.Log("공격성공");
                LivingEntity playerHealth = bewareOf.GetComponent<LivingEntity>();
                playerHealth.ApplyDamage(damageMessage);
                lastAttackTime = Time.time;

                if (playerHealth.IsDead) currentState = State.Wander;
                return Node.State.SUCCESS;
            }
            else
            {
                return Node.State.RUNNING;
            }
        }
        else
        {
            return Node.State.FAILURE;
        }

    }



    //구석에 몰려있는지 확인함. (위협 -> 공격으로 넘어가기 위한 변수)
    bool IsInCorner()
    {
        int blockedRayCount = 0;
        for (int i = 0; i < cornerRayCount; i++)
        {
            float angle = i * (360f / cornerRayCount);
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            rayDirections[i] = direction;  // Ray 방향 저장

            NavMeshHit hit;
            if (NavMesh.Raycast(transform.position, transform.position + direction * cornerDetectionRadius, out hit, NavMesh.AllAreas))
            {
                blockedRayCount++;
            }
        }
        return blockedRayCount > cornerRayCount / 2; // 절반 이상의 방향이 막혀 있으면 구석으로 판단
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