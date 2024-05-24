using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BackupAI : MonoBehaviour
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
    [SerializeField] float sporeDuration = 4f;
    Vector3 wanderDest;
    public bool setDesti = false;
    public Transform bewareOf;
    [SerializeField] GameObject head;
    private Transform pivot;
    public GameObject sporeParticle;
    [SerializeField] int cornerRayCount = 8;
    [SerializeField] float cornerDetectionRadius = 3f;
    [SerializeField] float attackDistance = 3f;
    [SerializeField] bool attackState = false;
    public GameObject Obj_Transform;

    void Start()
    {
        navMeshAgent = transform.parent.GetComponent<NavMeshAgent>();
        pivot = transform.parent;
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
        ActionNode setRunDest = new ActionNode(SetRunDest);
        ActionNode runFromPlayer = new ActionNode(RunFromPlayer);

        //공격 시퀀스의 children Node들
        ActionNode moveToPlayer = new ActionNode(MoveToPlayer);
        ActionNode attackPlayer = new ActionNode(AttackPlayer);

        //배회 시퀀스의 children Node들
        ActionNode setDest = new ActionNode(SetDest);
        ActionNode moveToDest = new ActionNode(MoveToDest);

        //셀렉터 노드에 들어갈 시퀀스 노드들
        SequenceNode threatSequence = new SequenceNode(new List<Node> { threaten, explodeSpore, setRunDest, runFromPlayer });
        SequenceNode attackSequence = new SequenceNode(new List<Node> { moveToPlayer, attackPlayer });
        SequenceNode wanderSequence = new SequenceNode(new List<Node> { setDest, moveToDest });
        topNode = new SelectorNode(new List<Node> { threatSequence, attackSequence, wanderSequence });
    }


    //[위협 시퀀스] 플레이어 응시
    private Node.State Threaten()
    {
        //위협 대기시간 3~5초 동안 위협
        //또는 플레이어 진짜 가까이 다가오면 도망감.
        if (bewareOf == null) return Node.State.FAILURE;

        if (attackState) return Node.State.FAILURE;

        if (Vector3.Distance(transform.position, bewareOf.position) < threatDistance)
        {
            if (!isThreatening)
            {
                setDesti = false;
                isThreatening = true;
                threatTime = Time.time;
                threatDuration = Random.Range(3f, 5f);
            }

            //위협하기
            if (Time.time - threatTime < threatDuration)
            {
                //플레이어가 더 가까이오면 도망
                if (Vector3.Distance(transform.position, bewareOf.position) < runDistance) return Node.State.SUCCESS;

                navMeshAgent.SetDestination(transform.position);
                pivot.LookAt(bewareOf.position);
                Debug.Log("위협 중");

                return Node.State.RUNNING;
            }
            else
            {
                Debug.Log("위협 끝");
                return Node.State.SUCCESS;
            }
        }
        else return Node.State.FAILURE;
    }

    //[위협 시퀀스] 포자 발사
    private Node.State ExplodeSpore()
    {
        //이거 포자 끝도 없이 생성될거 같음.
        //포자 발사 여부를 확률로 결정
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
        if (IsInCorner()) { attackState = true; return Node.State.FAILURE; }
        else return Node.State.SUCCESS;
    }

    //[위협 시퀀스] 도망목적지 설정
    private Node.State SetRunDest()
    {
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
        return Node.State.SUCCESS;
    }

    //[위협 시퀀스] 도망
    private Node.State RunFromPlayer()
    {
        if (Vector3.Distance(head.transform.position, wanderDest) <= 1f)
        {
            Debug.Log("도망 목적지 도착");
            setDesti = false;
            navMeshAgent.speed = defaultSpeed;
            navMeshAgent.acceleration = defaultAccel;
            navMeshAgent.angularSpeed = defaultAngle;
            isThreatening = false;
            bewareOf = null;
            return Node.State.SUCCESS;
        }
        else return Node.State.RUNNING;
    }

    //[공격 시퀀스] 플레이어에게 접근
    private Node.State MoveToPlayer()
    {
        if (bewareOf == null) return Node.State.FAILURE;

        if (!IsInCorner()) return Node.State.FAILURE;

        Obj_Transform.transform.position = head.transform.position;
        Obj_Transform.transform.rotation = head.transform.rotation;

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
        Obj_Transform.transform.position = head.transform.position;
        Obj_Transform.transform.rotation = head.transform.rotation;
        //Debug.Log(head.position + ", " + bewareOf.position + ", " + Vector3.Distance(head.position, bewareOf.position));

        if (Vector3.Distance(head.transform.position, bewareOf.position) <= attackDistance)
        {
            if (Time.time - lastAttackTime >= attackCooltime)
            {
                Debug.Log("공격성공");
                LivingEntity playerHealth = bewareOf.GetComponent<LivingEntity>();
                playerHealth.ApplyDamage(damageMessage);
                lastAttackTime = Time.time;
                attackState = false;
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

    //[배회 시퀀스] 목적지 설정
    private Node.State SetDest()
    {
        if (!setDesti)
        {
            Debug.Log("목적지 설정");
            wanderDest = RandomNavMeshMovement.RandomNavSphere(transform.position, wanderRadius, -1);
            navMeshAgent.SetDestination(wanderDest);
            setDesti = true;
        }
        return Node.State.SUCCESS;
    }

    //[배회 시퀀스] 이동
    private Node.State MoveToDest()
    {
        if (Vector3.Distance(head.transform.position, wanderDest) <= 1f)
        {
            Debug.Log("목적지 도착");
            setDesti = false;
            return Node.State.SUCCESS;
        }
        else return Node.State.RUNNING;
    }

    //구석에 몰려있는지 확인함. (위협 -> 공격으로 넘어가기 위한 변수)
    bool IsInCorner()
    {
        int blockedRayCount = 0;
        for (int i = 0; i < cornerRayCount; i++)
        {
            float angle = i * (360f / cornerRayCount);
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            NavMeshHit hit;
            if (NavMesh.Raycast(transform.position, transform.position + direction * cornerDetectionRadius, out hit, NavMesh.AllAreas))
            {
                blockedRayCount++;
            }
        }
        return blockedRayCount > cornerRayCount / 2; // 절반 이상의 방향이 막혀 있으면 구석으로 판단
    }

}
