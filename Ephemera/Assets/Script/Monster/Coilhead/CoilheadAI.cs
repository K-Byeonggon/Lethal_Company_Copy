using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CoilheadAI : MonsterAI
{
    private Node topNode;
    [SerializeField] public NavMeshAgent navMeshAgent;

    //Attack
    public Transform target;
    [SerializeField] float attackDistance = 1f;
    [SerializeField] float attackCooltime = 0.2f;
    private float lastAttackTime;
    private bool isCooltime => Time.time - lastAttackTime < attackCooltime;
    private DamageMessage damageMessage;

    //Wander
    [SerializeField] float wanderRadius = 30f;


    void Start()
    {
        
    }
    public override void OnStartServer()
    {
        enabled = true;
        navMeshAgent.enabled = true;
        MonsterReference.Instance.AddMonsterToList(gameObject);
        
        openDoorDelay = 3f;
        ConstructBehaviorTree();
        damageMessage = new DamageMessage();
        damageMessage.damage = 90;
        damageMessage.damager = gameObject;
    }
    
    private void ConstructBehaviorTree()
    {
        ActionNode checkAttackWill = new ActionNode(CheckAttackWill);
        ActionNode trackTarget = new ActionNode(TrackTarget);
        ActionNode attackTarget = new ActionNode(AttackTarget);

        ActionNode setDestination = new ActionNode(SetDestination);
        ActionNode wander = new ActionNode(Wander);

        SequenceNode attackSequence = new SequenceNode(new List<Node> { checkAttackWill, trackTarget, attackTarget });
        SequenceNode wanderSequence = new SequenceNode(new List<Node> { setDestination, wander });
        topNode = new SelectorNode(new List<Node> { attackSequence, wanderSequence } );
    }
    
    void Update()
    {
        if (isServer)
        {
            topNode.Evaluate();
        }

        //Test_BehaviourTree.Instance.nodeStatus.text = $"Current Node: {currentNodeName}";
    }

    //[공격 시퀀스] 공격 의지
    private Node.State CheckAttackWill()
    {
        currentNodeName = "CheckAttackWill";

        //플레이어를 발견했으면 대상 플레이어를 추적.
        if(target != null)
        {
            Debug.Log($"{transform.name}가 플레이어 발견함.");

            return Node.State.SUCCESS;
        }
        else return Node.State.FAILURE;
    }

    // 코일헤드는 쳐다봐지면 정지함.
    private void CheckBeWatched()
    {
        if (beWatched)
        {
            navMeshAgent.isStopped = true;
        }
        else
        {
            navMeshAgent.isStopped = false;
        }
    }

    //[공격 시퀀스] 추적
    private Node.State TrackTarget()
    {
        currentNodeName = "TrackTarget";
        CheckBeWatched();

        //추적 대상을 잃거나 추적 대상이 죽으면 실패
        if (target == null || target.GetComponent<LivingEntity>().IsDead)
        {
            return Node.State.FAILURE;
        }
        else if (Vector3.Distance(transform.position, target.position) <= attackDistance)
        {
            Debug.Log($"{transform.name}가 플레이어에게 다가가기 성공");

            return Node.State.SUCCESS;
        }
        else
        {
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

        if (!isCooltime && player != null && !player.IsDead)
        {
            Debug.Log($"{transform.name}가 플레이어 공격");
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

        CheckBeWatched();

        if (target != null)
        {
            return Node.State.FAILURE;
        }
        //목적지에 도달하면 SUCCESS 반환하고 다음 프레임에 새로운 목적지 설정
        else if (Vector3.Distance(transform.position, navMeshAgent.destination) <= .5f)
        {
            return Node.State.SUCCESS;
        }
        else
        {
            return Node.State.RUNNING;
        }
    }
}
