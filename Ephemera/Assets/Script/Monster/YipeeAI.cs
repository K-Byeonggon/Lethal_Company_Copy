using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class YipeeAI : MonsterAI
{
    private Node topNode;
    public Transform player;
    public Transform bewareOf;
    public float detectionRange = 10f;
    public float stoppingDistance = 2f;
    public UnityEngine.AI.NavMeshAgent navMeshAgent;

    [SerializeField] float attackDistance = 0.5f;
    public Transform nest;
    [SerializeField] Transform Item;
    public bool setDesti = false;
    public float wanderRadius = 10f;
    public Vector3 itemPos;
    public GameObject detectedItem;
    public bool itemFind = false;
    public bool itemHave = false;
    public bool isAttacked = false;

    private DamageMessage damageMessage;
    private YipeeHealth yipeeHealth;
    [SerializeField] float attackCooltime = 2f;
    private float lastAttackTime;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        ConstructBehaviorTree();

        yipeeHealth = GetComponent<YipeeHealth>();
        damageMessage = new DamageMessage();
        damageMessage.damage = 30;
        damageMessage.damager = gameObject;
    }

    void Update()
    {
        topNode.Evaluate();
        Debug.Log(itemHave);
    }

    private void ConstructBehaviorTree()
    {
        //죽음 시퀀스의 children Node
        ActionNode dead = new ActionNode(Dead);

        //공격 시퀀스의 children Node들
        ActionNode attackWill = new ActionNode(AttackWill);
        ActionNode moveToPlayer = new ActionNode(MoveToPlayer);
        ActionNode attackPlayer = new ActionNode(AttackPlayer);

        //배회 시퀀스의 children Node들
        ActionNode setDest = new ActionNode(SetDest);
        ActionNode moveToDest = new ActionNode(MoveToDest);

        //탐색 시퀀스의 children Node들
        ActionNode setDestToScrap = new ActionNode(SetDestToScrap);
        ActionNode moveToScrap = new ActionNode(MoveToScrap);
        ActionNode getScrap = new ActionNode(GetScrap);
        ActionNode moveToNest = new ActionNode(MoveToNest);
        ActionNode setScrap = new ActionNode(SetScrap);

        //위협 시퀀스의 children Node
        ActionNode threathen = new ActionNode(Threaten);

        //셀렉터 노드에 들어갈 시퀀스 노드들
        SequenceNode attackSequence = new SequenceNode(new List<Node> { attackWill, moveToPlayer, attackPlayer });
        SequenceNode wanderSequence = new SequenceNode(new List<Node> { setDest, moveToDest });
        SequenceNode detectSequence = new SequenceNode(new List<Node> { setDestToScrap, moveToScrap, getScrap, moveToNest, setScrap });
        topNode = new SelectorNode(new List<Node> { dead, attackSequence, threathen, wanderSequence, detectSequence, });
    }

    //죽음 시퀀스 노드
    private Node.State Dead()
    {
        if (yipeeHealth.IsDead)
        {
            detectedItem.transform.parent = null;
            navMeshAgent.SetDestination(transform.position);
            return Node.State.SUCCESS;
        }
        else return Node.State.FAILURE;
    }

    //공격의지 활성화(공격 시퀀스)
    private Node.State AttackWill()
    {
        //1. 플레이어에게 공격 받았는지 bool변수 확인(YipeeHealth에서)
        if (isAttacked) 
        { 
            //YipeeHealth에서 플레이어 갱신. 
            return Node.State.SUCCESS; 
        }
        //2. 플레이어에가 근처에 오래 있었는지 확인
        else if (false) { /*플레이어 Transform 갱신*/ return Node.State.SUCCESS; }
        //3. 플레이어가 둥지의 폐품 훔쳐간것을 봄.
        else if(false) { /*플레이어 Transform 갱신*/ return Node.State.SUCCESS; }
        else return Node.State.FAILURE;
    }

    //플레이어에게 접근(공격 시퀀스)
    private Node.State MoveToPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > attackDistance)
        {
            detectedItem.transform.parent = null;
            navMeshAgent.SetDestination(player.position);
            return Node.State.RUNNING;
        }
        else
        {
            return Node.State.SUCCESS;
        }
    }

    //플레이어를 공격(공격 시퀀스)
    private Node.State AttackPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            // 공격 로직 수행
            if (Time.time - lastAttackTime >= attackCooltime)
            {
                LivingEntity playerHealth = player.GetComponent<LivingEntity>();
                playerHealth.ApplyDamage(damageMessage);
                lastAttackTime = Time.time;
                if (playerHealth.dead) isAttacked = false;

                return Node.State.FAILURE;
            }
        }
        return Node.State.SUCCESS;
    }

    //위협 시퀀스 노드
    private Node.State Threaten()
    {
        if (bewareOf == null || itemHave) return Node.State.FAILURE;
        if (Vector3.Distance(bewareOf.position, nest.position) < 2f)
        {
            if(Vector3.Distance(transform.position, bewareOf.position) < 2f)
            {
                navMeshAgent.SetDestination(transform.position);
                transform.LookAt(bewareOf.position);
                Debug.Log("위협하는 동작");
                return Node.State.SUCCESS;
            }
            else return Node.State.RUNNING;
        }
        else return Node.State.RUNNING;
    }

    //랜덤 목적지 설정(한번만 실행)(돌아다니기 시퀀스)
    private Node.State SetDest()
    {
        Debug.Log("SetDest");
        if (itemFind) return Node.State.FAILURE;        //아이템을 찾아서 아이템을 추적하는 상태면,
        else if (setDesti) return Node.State.SUCCESS;   //이미 목적지 설정이 되어있으면,
        else
        {
            Vector3 newPos = RandomNavMeshMovement.RandomNavSphere(nest.position, wanderRadius, -1);
            navMeshAgent.SetDestination(newPos);
            setDesti = true;
            return Node.State.SUCCESS;
        }
    }


    //목적지로 이동(돌아다니기 시퀀스)
    private Node.State MoveToDest()
    {
        Debug.Log("MoveToDest");

        if (itemFind) return Node.State.FAILURE;

        //목적지에 도달함.
        else if (Vector3.Distance(transform.position, navMeshAgent.destination) <= 1f)
        {
            setDesti = false;
            return Node.State.SUCCESS;
        }
        else return Node.State.RUNNING;
    }

    //폐품으로 목적지 수정(탐색 시퀀스)
    private Node.State SetDestToScrap()
    {
        Debug.Log("SetDestToScrap");
        if (itemFind)
        {
            navMeshAgent.SetDestination(detectedItem.transform.position);
            return Node.State.SUCCESS;
        }
        else return Node.State.FAILURE;
    }

    //폐품으로 이동(탐색 시퀀스)
    private Node.State MoveToScrap()
    {
        Debug.Log("MoveToScrap");
        if (itemFind)
        {
            //도중에 아이템이 내가 아닌 누군가에게 주워지면 FAILURE.
            if (detectedItem.transform.parent != null && detectedItem.transform.parent != transform.GetChild(1)) return Node.State.FAILURE;

            if (Vector3.Distance(transform.position, navMeshAgent.destination) <= 0.5f)
            {
                return Node.State.SUCCESS;
            }
            else
            {
                Debug.Log("RUNNING이 의미가 있나?");
                return Node.State.RUNNING;
            }
        }
        else return Node.State.FAILURE;
    }


    //폐품 수집(탐색 시퀀스)
    private Node.State GetScrap()
    {
        Debug.Log("GetScrap");

        if (!itemHave)
        {
            //페품 들기

            //도중에 아이템이 내가 아닌 누군가에게 주워지면 FAILURE.
            if (detectedItem.transform.parent != null && detectedItem.transform.parent != transform) return Node.State.FAILURE;

            Debug.Log("들어올림");
            detectedItem.transform.position = transform.GetChild(1).position;
            detectedItem.transform.SetParent(transform.GetChild(1));
            itemHave = true;

            //둥지로 목적지 설정.
            navMeshAgent.SetDestination(nest.position);
            setDesti = true;
            return Node.State.SUCCESS;
        }
        else if (itemHave) return Node.State.SUCCESS;
        else return Node.State.FAILURE;

    }

    //폐품 들고 둥지로(탐색 시퀀스)
    private Node.State MoveToNest()
    {
        Debug.Log("MoveToNest");
        if (Vector3.Distance(transform.position, navMeshAgent.destination) <= 0.5f)
        {
            return Node.State.SUCCESS;
        }
        else { return Node.State.RUNNING; }
    }

    //페품 내려놓기(탐색 시퀀스)
    private Node.State SetScrap()
    {
        if(itemHave)
        {
            Debug.Log("내려놓음");
            detectedItem.transform.parent = null;
            setDesti = false;
            itemFind = false;
            itemHave = false;
            return Node.State.FAILURE;
        }
        else return Node.State.SUCCESS;
    }
}