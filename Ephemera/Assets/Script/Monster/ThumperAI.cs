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

    private DamageMessage damageMessage;
    private ThumperHealth thumperHealth;
    [SerializeField] float attackCooltime = 0.33f;
    private float lastAttackTime;

    public Transform target;
    [SerializeField] float defaultSpeed = 2.5f;
    [SerializeField] float defaultAnglerSpeed = 120f;
    [SerializeField] float defaultAccel = 8f;
    [SerializeField] float rushSpeed = 16f;
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
        //플레이어를 발견했으면 목적지를 한번 자신으로 바꾸고, 다시 대상 플레이어가 있던 위치를 목표로 지정. (현재 스피드를 갱신)
        if (sawPlayer)
        {
            //최대 속도가 증가하고, 각속도가 줄어든다.
            Debug.Log("플레이어 봤다.");
            navMeshAgent.SetDestination(transform.position);
            navMeshAgent.speed = rushSpeed;
            navMeshAgent.angularSpeed = rushAnglerSpeed;
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

    //[공격 시퀀스] 목표를 향해 이동. 
    private Node.State MoveToPlayer()
    {
        Debug.Log(Vector3.Distance(transform.position, target.position));
        Debug.Log(transform.name + transform.position + ", " + target.name + target.position);
        if (Vector3.Distance(transform.position, target.position) <= attackDistance)
        {
            Debug.Log("다가갔다.");
            return Node.State.SUCCESS;
        }
        else if (hitWall)
        {
            Debug.Log("벽에 박음");
            return Node.State.FAILURE;
            //공격 실패 후 배회 시퀀스로
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
                playerHealth.ApplyDamage(damageMessage);
                lastAttackTime = Time.time;

                return Node.State.SUCCESS;
            }
        }
        return Node.State.SUCCESS;
    }

    //[배회 시퀀스] 목적지 설정
    private Node.State SetDest()
    {
        Debug.Log(setDesti);

        if (sawPlayer) return Node.State.FAILURE;        //플레이어 추적하는 상태면,
        else if (setDesti) return Node.State.SUCCESS;   //이미 목적지 설정이 되어있으면,
        else
        {
            Vector3 newPos = RandomNavMeshMovement.RandomNavSphere(transform.position, wanderRadius, -1);
            navMeshAgent.SetDestination(newPos);
            setDesti = true;
            return Node.State.SUCCESS;
        }
    }

    //[배회 시퀀스] 목적지 이동
    private Node.State MoveToDest()
    {
        if (sawPlayer) return Node.State.FAILURE;

        //목적지에 도달함.
        else if (Vector3.Distance(transform.position, navMeshAgent.destination) <= .5f)
        {
            setDesti = false;
            return Node.State.SUCCESS;
        }
        else return Node.State.RUNNING;
    }
}