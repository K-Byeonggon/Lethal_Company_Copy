using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SporeLizardAI : MonoBehaviour
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
    private Transform head;
    private Transform pivot;
    public GameObject sporeParticle;

    void Start()
    {
        navMeshAgent = transform.parent.GetComponent<NavMeshAgent>();
        head = transform.GetChild(0);
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
        //공격 시퀀스의 children Node들

        //도망 시퀀스의 children Node

        //위협 시퀀스의 children Node
        ActionNode threaten = new ActionNode(Threaten);
        ActionNode explodeSpore = new ActionNode(ExplodeSpore);
        ActionNode setRunDest = new ActionNode(SetRunDest);
        ActionNode runFromPlayer = new ActionNode(RunFromPlayer);

        //배회 시퀀스의 children Node들
        ActionNode setDest = new ActionNode(SetDest);
        ActionNode moveToDest = new ActionNode(MoveToDest);

        //셀렉터 노드에 들어갈 시퀀스 노드들
        SequenceNode threatSequence = new SequenceNode(new List<Node> { threaten, explodeSpore, setRunDest, runFromPlayer });
        SequenceNode wanderSequence = new SequenceNode(new List<Node> { setDest, moveToDest });
        topNode = new SelectorNode(new List<Node> { threatSequence, wanderSequence });
    }


    //[위협 시퀀스] 플레이어 응시
    private Node.State Threaten()
    {
        //위협 대기시간 3~5초 동안 위협
        //또는 플레이어 진짜 가까이 다가오면 도망감.
        if(bewareOf == null) return Node.State.FAILURE;

        if (Vector3.Distance(transform.position, bewareOf.position) < threatDistance)
        {
            if(!isThreatening)
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
    
    //[위협 시퀀스] 포자 발사 & 도망목
    private Node.State ExplodeSpore()
    {
        //이거 포자 끝도 없이 생성될거 같음.
        //포자 발사 여부를 확률로 결정
        if (Random.value <= sporePercentage) 
        {
            if(haveSpore)
            {
                Debug.Log("포자 발사");
                haveSpore = false;
                Instantiate(sporeParticle, transform.position, Quaternion.identity);
            }
        }
        return Node.State.SUCCESS;
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
        if (Vector3.Distance(head.position, wanderDest) <= 1f)
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
        if (Vector3.Distance(head.position, wanderDest) <= 1f)
        {
            Debug.Log("목적지 도착");
            setDesti = false;
            return Node.State.SUCCESS;
        }
        else return Node.State.RUNNING;
    }

}
