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
        //�������� óġ �Ұ��� ����. ü�µ� ������ ����.

        //���� ������
        ActionNode stop = new ActionNode(Stop);

        //���� �������� children Node��
        ActionNode attackWill = new ActionNode(AttackWill);
        ActionNode moveToPlayer = new ActionNode(MoveToPlayer);
        ActionNode attackPlayer = new ActionNode(AttackPlayer);

        //��ȸ �������� children Node��
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

    //[���� ������] ����
    private Node.State Stop()
    {
        //�÷��̾ ���� ������ SUCCESS�� ����.
        if(beWatched)
        {
            navMeshAgent.SetDestination(transform.position);
            return Node.State.SUCCESS;
        }
        else { return Node.State.FAILURE; }
        //�ƴϸ� FAILURE
    }

    //[���� ������] ���� ����
    private Node.State AttackWill()
    {
        //�÷��̾ �߰������� �ӵ��� �������� ��� �÷��̾ ��ǥ�� ����.
        if(sawPlayer)
        {
            Debug.Log("�÷��̾� �ô�.");
            navMeshAgent.SetDestination(target.position);
            return Node.State.SUCCESS;
        }
        else return Node.State.FAILURE;
    }

    //[���� ������] �÷��̾ ���� �̵�
    private Node.State MoveToPlayer()
    {
        Debug.Log("MoveToPlayer");
        Debug.Log(transform.name + transform.position + ", " + target.name + target.position);
        Debug.Log(Vector3.Distance(transform.position, target.position));
        if(Vector3.Distance(transform.position, target.position) <= attackDistance)
        {
            Debug.Log("�ٰ�����.");
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
                playerHealth.ApplyDamage(damageMessage);
                lastAttackTime = Time.time;

                return Node.State.FAILURE;
            }
        }
        return Node.State.SUCCESS;
    }
    

    //[��ȸ ������] ������ ����
    private Node.State SetDest()
    {
        if (sawPlayer) return Node.State.FAILURE;        //�÷��̾� �����ϴ� ���¸�,
        else if (setDesti) return Node.State.SUCCESS;   //�̹� ������ ������ �Ǿ�������,
        else
        {
            Vector3 newPos = RandomNavMeshMovement.RandomNavSphere(transform.position, wanderRadius, -1);
            navMeshAgent.SetDestination(newPos);
            setDesti = true;
            return Node.State.SUCCESS;
        }
    }

    //[��ȸ ������] ������ �̵�
    private Node.State MoveToDest()
    {
        if (sawPlayer) return Node.State.FAILURE;

        //�������� ������.
        else if (Vector3.Distance(transform.position, navMeshAgent.destination) <= .5f)
        {
            setDesti = false;
            return Node.State.SUCCESS;
        }
        else return Node.State.RUNNING;
    }
}
