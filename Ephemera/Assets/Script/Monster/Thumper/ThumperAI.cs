using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ThumperAI : MonsterAI
{
    //public Transform player;
    //public bool sawPlayer = false;
    //public Vector3 destination;
    //public bool setDesti = false;
    //public bool isAttacked = false;
    //public bool wandering = false;

    private Node topNode;
    private string currentNodeName = default;
    public bool hitWall = false;

    public NavMeshAgent navMeshAgent;
    private ThumperHealth thumperHealth;

    public Transform target;
    
    //Attack
    public bool isWillingToAttack = false;
    [SerializeField] float attackDistance = 1f;
    [SerializeField] float attackCooltime = 0.33f;
    private float lastAttackTime;
    private bool isCooltime => Time.time - lastAttackTime < attackCooltime;
    private DamageMessage damageMessage;

    //default parameter
    [SerializeField] float defaultSpeed = 2.5f;
    [SerializeField] float defaultAnglerSpeed = 120f;
    [SerializeField] float defaultAccel = 8f;

    //rush parameter
    [SerializeField] float rushSpeed = 24f;
    [SerializeField] float rushAnglerSpeed = 10f;

    //wander
    public float wanderRadius = 10f;

    private void Awake()
    {
        //navMeshAgent = GetComponent<NavMeshAgent>();
        thumperHealth = GetComponent<ThumperHealth>();
        ConstructBehaviorTree();
    }

    public override void OnStartServer()
    {
        enabled = true;
        navMeshAgent.enabled = true;
        MonsterReference.Instance.AddMonsterToList(transform.parent.gameObject);
        

        damageMessage = new DamageMessage();
        damageMessage.damage = 40;
        damageMessage.damager = gameObject;

        //행동 트리 테스트 UI
        UIController.Instance.SetActivateUI(typeof(UI_Game));
    }

    private void Update()
    {
        if (isServer)
        {
            topNode.Evaluate();
        }

        Test_BehaviourTree.Instance.nodeStatus.text = $"Current Node: {currentNodeName}";
    }

    private void ConstructBehaviorTree()
    {
        ActionNode dead = new ActionNode(Dead);

        //공격 시퀀스
        ActionNode checkAttackWill = new ActionNode(CheckAttackWill);
        ActionNode trackTarget = new ActionNode(TrackTarget);
        ActionNode attackTarget = new ActionNode(AttackTarget);

        //배회 시퀀스
        ActionNode setDestination = new ActionNode(SetDestination);
        ActionNode wander = new ActionNode(Wander);

        //시퀀스 노드들
        SequenceNode attackSequence = new SequenceNode(new List<Node> { checkAttackWill, trackTarget, attackTarget });
        SequenceNode wanderSequence = new SequenceNode(new List<Node> { setDestination, wander });

        //셀렉터 노드(탑 노드)
        topNode = new SelectorNode(new List<Node> { dead, attackSequence, wanderSequence });
    }

    //���� ������ ���
    private Node.State Dead()
    {
        currentNodeName = "Dead";
        if (thumperHealth.IsDead)
        {
            navMeshAgent.SetDestination(transform.position);
            return Node.State.SUCCESS;
        }
        else return Node.State.FAILURE;
    }

    //[공격 시퀀스] 공격 의지 검사

    private Node.State CheckAttackWill()
    {
        currentNodeName = "CheckAttackWill";
        if (isWillingToAttack)
        {
            Debug.Log($"[공격 시퀀스] {transform.name}가 공격의지가 있음.");
            navMeshAgent.speed = rushSpeed;
            navMeshAgent.angularSpeed = rushAnglerSpeed;
            navMeshAgent.destination = target.position;

            return Node.State.SUCCESS;
        }
        else
        {
            Debug.Log($"[공격 시퀀스] {transform.name}가 공격할 생각 없음.");
            SetNavmeshDefault();

            return Node.State.FAILURE;
        }
    }

    private void SetNavmeshDefault()
    {
        navMeshAgent.speed = defaultSpeed;
        navMeshAgent.angularSpeed = defaultAnglerSpeed;
        navMeshAgent.acceleration = defaultAccel;
    }

    //[공격 시퀀스] 추적
    private Node.State TrackTarget()
    {
        currentNodeName = "TrackTarget";
        if (hitWall)
        {
            Debug.Log($"[공격 시퀀스] {transform.name}가 벽에 부딪힘.");
            return Node.State.FAILURE;
        }
        //덤퍼는 타겟을 추적하지 않고, 타겟이 있던자리를 추적한다.
        else if(Vector3.Distance(transform.position, navMeshAgent.destination) <= attackDistance)
        {
            Debug.Log($"[공격 시퀀스] {transform.name}가 목적지 까지 돌진 성공.");
            return Node.State.SUCCESS;
        }
        else
        {
            Debug.Log($"[공격 시퀀스] {transform.name}가 돌진 중.");
            return Node.State.RUNNING;
        }
    }

    private Node.State AttackTarget()
    {
        currentNodeName = "AttackTarget";
        //돌진 끝나고 타겟이 공격범위 안에 없으면 공격 의지 끄기
        if (Vector3.Distance(transform.position, target.position) > attackDistance)
        {
            Debug.Log($"[공격 시퀀스] {transform.name}가 타깃을 찾지 못함.");
            isWillingToAttack = false;
            SetNavmeshDefault();

            return Node.State.FAILURE;
        }
        
        LivingEntity playerHealth = target.GetComponent<LivingEntity>();

        //공격 쿨타임이 아니고 타겟이 살아있으면
        if (!isCooltime && playerHealth != null && !playerHealth.IsDead)
        {
            Debug.Log($"[공격 시퀀스] {transform.name}가 타깃을 공격");
            playerHealth.ApplyDamage(damageMessage);
            lastAttackTime = Time.time;

            return Node.State.SUCCESS;
        }

        //아니면 공격 의지 끄기
        Debug.Log($"[공격 시퀀스] {transform.name}가 공격 쿨타임임.");
        isWillingToAttack = false;
        SetNavmeshDefault();

        return Node.State.FAILURE;
    }

    //[배회 시퀀스] 목적지 설정
    private Node.State SetDestination()
    {
        currentNodeName = "SetDestination";
        Debug.Log($"[배회 시퀀스] {transform.name}가 목적지를 설정함.");
        Vector3 newPos = RandomNavMeshMovement.RandomNavSphere(transform.position, wanderRadius, -1);

        navMeshAgent.SetDestination(newPos);

        return Node.State.SUCCESS;
    }

    private Node.State Wander()
    {
        currentNodeName = "Wander";
        //공격 의지 활성화되면 FAILURE 반환하고 다음 프레임에 공격
        if (isWillingToAttack)
        {
            Debug.Log($"[배회 시퀀스] {transform.name}가 공격의지가 생김.");
            return Node.State.FAILURE;
        }
        //목적지 도착하면 SUCCESS 반환하고 다음 프레임에 새로운 목적지 설정
        else if (Vector3.Distance(transform.position, navMeshAgent.destination) <= .5f)
        {
            Debug.Log($"[배회 시퀀스] {transform.name}가 목적지에 도착함.");
            return Node.State.SUCCESS;
        }
        else
        {
            Debug.Log($"[배회 시퀀스] {transform.name}가 목적지로 가는중..");
            return Node.State.RUNNING;
        }
    }

    //이전 코드
    /*
    private Node.State AttackWill()
    {
        if (setDesti)
        {
            Debug.Log("�÷��̾� �ô�.");
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

    private Node.State MoveToPlayer()
    {
        //Debug.Log(Vector3.Distance(transform.position, target.position));
        //Debug.Log(transform.name + transform.position + ", " + target.name + target.position);
        if (Vector3.Distance(transform.position, destination) <= attackDistance)
        {
            Debug.Log("�ٰ�����.");
            return Node.State.SUCCESS;
        }
        else if (hitWall)
        {
            return Node.State.SUCCESS;
        }
        else return Node.State.RUNNING;
    }

    private Node.State AttackPlayer()
    {
        if (Vector3.Distance(transform.position, target.position) <= attackDistance)
        {
            if (Time.time - lastAttackTime >= attackCooltime)
            {
                LivingEntity playerHealth = target.GetComponent<LivingEntity>();
                
                if(playerHealth.IsDead) { return Node.State.FAILURE; }

                playerHealth.ApplyDamage(damageMessage);
                lastAttackTime = Time.time;

                return Node.State.SUCCESS;
            }
        }
        return Node.State.FAILURE;
    }

    private Node.State SetDest()
    {
        if (!wandering)
        {
            Vector3 newPos = RandomNavMeshMovement.RandomNavSphere(transform.position, wanderRadius, -1);
            navMeshAgent.SetDestination(newPos);
            wandering = true;
        }
        setDesti = false;
        return Node.State.SUCCESS;
    }

    private Node.State MoveToDest()
    {
        if (Vector3.Distance(transform.position, navMeshAgent.destination) <= .5f)
        {
            wandering = false;
            return Node.State.SUCCESS;
        }
        else return Node.State.RUNNING;
    }*/
}