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

    [SerializeField] public UnityEngine.AI.NavMeshAgent navMeshAgent;

    [SerializeField] float attackDistance = 1f;
    public bool setDesti = false;
    public float wanderRadius = 10f;
    public bool isAttacked = false;
    public bool wandering = false;

    private DamageMessage damageMessage;
    [SerializeField] private ThumperHealth thumperHealth;
    [SerializeField] float attackCooltime = 0.33f;
    private float lastAttackTime;

    public Transform target;
    [SerializeField] float defaultSpeed = 2.5f;
    [SerializeField] float defaultAnglerSpeed = 120f;
    [SerializeField] float defaultAccel = 8f;
    [SerializeField] float rushSpeed = 24f;
    [SerializeField] float rushAnglerSpeed = 10f;


    public override void OnStartServer()
    {
        enabled = true;
        navMeshAgent.enabled = true;
        MonsterReference.Instance.AddMonsterToList(gameObject);
        
        ConstructBehaviorTree();

        damageMessage = new DamageMessage();
        damageMessage.damage = 40;
        damageMessage.damager = gameObject;
    }

    void Update()
    {
        if (isServer)
        {
            topNode.Evaluate();
        }
    }

    private void ConstructBehaviorTree()
    {
        //���� �������� children Node
        ActionNode dead = new ActionNode(Dead);

        //���� ������
        ActionNode attackWill = new ActionNode(AttackWill);
        ActionNode moveToPlayer = new ActionNode(MoveToPlayer);
        ActionNode attackPlayer = new ActionNode(AttackPlayer);

        
        //��ȸ �������� children Node��
        ActionNode setDest = new ActionNode(SetDest);
        ActionNode moveToDest = new ActionNode(MoveToDest);

        SequenceNode attackSequence = new SequenceNode(new List<Node> { attackWill, moveToPlayer, attackPlayer });
        SequenceNode wanderSequence = new SequenceNode(new List<Node> { setDest, moveToDest });
        topNode = new SelectorNode(new List<Node> { dead, attackSequence, wanderSequence });

    }

    //���� ������ ���
    private Node.State Dead()
    {
        if (thumperHealth.IsDead)
        {
            navMeshAgent.SetDestination(transform.position);
            return Node.State.SUCCESS;
        }
        else return Node.State.FAILURE;
    }

    //[���� ������] ���� ������ ����.
    private Node.State AttackWill()
    {
        //�÷��̾ �߰������� Ȥ�� ���ݹ޾�����, �� ��ġ�� ��ǥ�� ����.
        if (setDesti)
        {
            //�ִ� �ӵ��� �����ϰ�, ���ӵ��� �پ���.
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

    //[���� ������] �������� ���� �̵�. 
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
            //�ƹ����� �΋H���� ���ݳ��� �Ѿ. ���� ���� ���δ� ���� ��忡�� ����.
            return Node.State.SUCCESS;
        }
        else return Node.State.RUNNING;
    }

    //[���� ������] �÷��̾� ����.
    private Node.State AttackPlayer()
    {
        if (Vector3.Distance(transform.position, target.position) <= attackDistance)
        {
            if (Time.time - lastAttackTime >= attackCooltime)
            {
                Debug.Log("�����Ѵ�.");
                LivingEntity playerHealth = target.GetComponent<LivingEntity>();
                
                //�÷��̾� �׾����� ��ȸ ��������
                if(playerHealth.IsDead) { return Node.State.FAILURE; }

                //�ƴϸ� ������ ����.
                playerHealth.ApplyDamage(damageMessage);
                lastAttackTime = Time.time;

                return Node.State.SUCCESS;
            }
        }
        return Node.State.FAILURE;
    }

    //[��ȸ ������] ������ ����
    private Node.State SetDest()
    {
        //Debug.Log("��ȸ ������");

        if (!wandering)
        {
            Vector3 newPos = RandomNavMeshMovement.RandomNavSphere(transform.position, wanderRadius, -1);
            navMeshAgent.SetDestination(newPos);
            wandering = true;
        }
        setDesti = false;
        return Node.State.SUCCESS;
    }

    //[��ȸ ������] ������ �̵�
    private Node.State MoveToDest()
    {
        //�������� ������.
        if (Vector3.Distance(transform.position, navMeshAgent.destination) <= .5f)
        {
            wandering = false;
            return Node.State.SUCCESS;
        }
        else return Node.State.RUNNING;
    }
}