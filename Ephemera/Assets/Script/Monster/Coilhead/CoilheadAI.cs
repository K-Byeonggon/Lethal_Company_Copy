using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CoilheadAI : MonsterAI
{
    private Node topNode;
    public UnityEngine.AI.NavMeshAgent navMeshAgent;
    private DamageMessage damageMessage;
    public bool sawPlayer = false;
    public Transform target;
    [SerializeField] float attackDistance = 1f;
    private float lastAttackTime;
    [SerializeField] float attackCooltime = 0.2f;
    [SerializeField] float wanderRadius = 10f;
    public bool setDesti = false;


    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        ConstructBehaviorTree();

        damageMessage = new DamageMessage();
        damageMessage.damage = 90;
        damageMessage.damager = gameObject;
    }
    
    private void ConstructBehaviorTree()
    {
        //코일헤드는 처치 불가능 몬스터. 체력도 죽음도 없음.

        //정지 시퀀스
        ActionNode stop = new ActionNode(Stop);

        //공격 시퀀스의 children Node들
        ActionNode attackWill = new ActionNode(AttackWill);
        ActionNode moveToPlayer = new ActionNode(MoveToPlayer);
        ActionNode attackPlayer = new ActionNode(AttackPlayer);

        //배회 시퀀스의 children Node들
        ActionNode setDest = new ActionNode(SetDest);
        ActionNode moveToDest = new ActionNode(MoveToDest);

        SequenceNode attackSequence = new SequenceNode(new List<Node> { attackWill, moveToPlayer, attackPlayer });
        SequenceNode wanderSequence = new SequenceNode(new List<Node> { setDest, moveToDest });
        topNode = new SelectorNode(new List<Node> { stop, attackSequence, wanderSequence } );
    }

    void Update()
    {
        if (isServer)
        {
            topNode.Evaluate();
        }
    }

    //[정지 시퀀스] 정지
    private Node.State Stop()
    {
        //플레이어가 보고 있으면 SUCCESS로 정지.
        if(beWatched)
        {
            navMeshAgent.SetDestination(transform.position);
            return Node.State.SUCCESS;
        }
        else { return Node.State.FAILURE; }
        //아니면 FAILURE
    }

    //[공격 시퀀스] 공격 의지
    private Node.State AttackWill()
    {
        //플레이어를 발견했으면 속도가 빨라지고 대상 플레이어를 목표로 지정.
        if(sawPlayer)
        {
            Debug.Log("플레이어 봤다.");
            navMeshAgent.SetDestination(target.position);
            return Node.State.SUCCESS;
        }
        else return Node.State.FAILURE;
    }

    //[공격 시퀀스] 플레이어를 향해 이동
    private Node.State MoveToPlayer()
    {
        Debug.Log("MoveToPlayer");
        Debug.Log(transform.name + transform.position + ", " + target.name + target.position);
        Debug.Log(Vector3.Distance(transform.position, target.position));
        if(Vector3.Distance(transform.position, target.position) <= attackDistance)
        {
            Debug.Log("다가갔다.");
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
                playerHealth.ApplyDamage(damageMessage);
                lastAttackTime = Time.time;

                return Node.State.FAILURE;
            }
        }
        return Node.State.SUCCESS;
    }
    

    //[배회 시퀀스] 목적지 설정
    private Node.State SetDest()
    {
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
