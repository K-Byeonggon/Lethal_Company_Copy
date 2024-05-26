using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ThumperAI : MonsterAI
{
    private Node topNode;
    public Transform player;
    public bool sawPlayer = false;
    public Vector3 destination;
    public bool hitWall = false;

    public UnityEngine.AI.NavMeshAgent navMeshAgent;

    [SerializeField] float attackDistance = 1f;
    public bool setDesti = false;
    public float wanderRadius = 10f;
    public bool isAttacked = false;
    public bool wandering = false;

    private DamageMessage damageMessage;
    private ThumperHealth thumperHealth;
    [SerializeField] float attackCooltime = 0.33f;
    private float lastAttackTime;

    public Transform target;
    [SerializeField] float defaultSpeed = 2.5f;
    [SerializeField] float defaultAnglerSpeed = 120f;
    [SerializeField] float defaultAccel = 8f;
    [SerializeField] float rushSpeed = 24f;
    [SerializeField] float rushAnglerSpeed = 10f;


    void Start()
    {
        navMeshAgent = transform.parent.GetComponent<NavMeshAgent>();
        ConstructBehaviorTree();

        thumperHealth = GetComponent<ThumperHealth>();
        damageMessage = new DamageMessage();
        damageMessage.damage = 40;
        damageMessage.damager = gameObject;
    }

    void Update()
    {
        topNode.Evaluate();
    }

    private void ConstructBehaviorTree()
    {
        //죽음 시퀀스의 children Node
        ActionNode dead = new ActionNode(Dead);

        //공격 시퀀스
        ActionNode attackWill = new ActionNode(AttackWill);
        ActionNode moveToPlayer = new ActionNode(MoveToPlayer);
        ActionNode attackPlayer = new ActionNode(AttackPlayer);

        
        //배회 시퀀스의 children Node들
        ActionNode setDest = new ActionNode(SetDest);
        ActionNode moveToDest = new ActionNode(MoveToDest);

        SequenceNode attackSequence = new SequenceNode(new List<Node> { attackWill, moveToPlayer, attackPlayer });
        SequenceNode wanderSequence = new SequenceNode(new List<Node> { setDest, moveToDest });
        topNode = new SelectorNode(new List<Node> { dead, attackSequence, wanderSequence });

    }

    //죽음 시퀀스 노드
    private Node.State Dead()
    {
        if (thumperHealth.IsDead)
        {
            navMeshAgent.SetDestination(transform.position);
            return Node.State.SUCCESS;
        }
        else return Node.State.FAILURE;
    }

    //[공격 시퀀스] 돌진 목적지 설정.
    private Node.State AttackWill()
    {
        //플레이어를 발견했으면 혹은 공격받았으면, 그 위치를 목표로 지정.
        if (setDesti)
        {
            //최대 속도가 증가하고, 각속도가 줄어든다.
            Debug.Log("플레이어 봤다.");
            //navMeshAgent.SetDestination(transform.position);
            navMeshAgent.speed = rushSpeed;
            //navMeshAgent.angularSpeed = rushAnglerSpeed;
            navMeshAgent.acceleration = 8f;
            navMeshAgent.SetDestination(destination);
            return Node.State.SUCCESS;
        }
        else
        {
            navMeshAgent.speed = defaultSpeed;
            navMeshAgent.angularSpeed = defaultAnglerSpeed;
            navMeshAgent.acceleration = defaultAccel;
            return Node.State.FAILURE;
        }
    }

    //[공격 시퀀스] 목적지를 향해 이동. 
    private Node.State MoveToPlayer()
    {
        //Debug.Log(Vector3.Distance(transform.position, target.position));
        //Debug.Log(transform.name + transform.position + ", " + target.name + target.position);
        if (Vector3.Distance(transform.position, destination) <= attackDistance)
        {
            Debug.Log("다가갔다.");
            return Node.State.SUCCESS;
        }
        else if (hitWall)
        {
            //아무데나 부딫히면 공격노드로 넘어감. 공격 실패 여부는 공격 노드에서 결정.
            return Node.State.SUCCESS;
        }
        else return Node.State.RUNNING;
    }

    //[공격 시퀀스] 플레이어 공격.
    private Node.State AttackPlayer()
    {
        if (Vector3.Distance(transform.position, target.position) <= attackDistance)
        {
            if (Time.time - lastAttackTime >= attackCooltime)
            {
                Debug.Log("공격한다.");
                LivingEntity playerHealth = target.GetComponent<LivingEntity>();
                
                //플레이어 죽었으면 배회 시퀀스로
                if(playerHealth.IsDead) { return Node.State.FAILURE; }

                //아니면 데미지 적용.
                playerHealth.ApplyDamage(damageMessage);
                lastAttackTime = Time.time;

                return Node.State.SUCCESS;
            }
        }
        return Node.State.FAILURE;
    }

    //[배회 시퀀스] 목적지 설정
    private Node.State SetDest()
    {
        //Debug.Log("배회 시퀀스");

        if (!wandering)
        {
            Vector3 newPos = RandomNavMeshMovement.RandomNavSphere(transform.position, wanderRadius, -1);
            navMeshAgent.SetDestination(newPos);
            wandering = true;
        }
        setDesti = false;
        return Node.State.SUCCESS;
    }

    //[배회 시퀀스] 목적지 이동
    private Node.State MoveToDest()
    {
        //목적지에 도달함.
        if (Vector3.Distance(transform.position, navMeshAgent.destination) <= .5f)
        {
            wandering = false;
            return Node.State.SUCCESS;
        }
        else return Node.State.RUNNING;
    }
}