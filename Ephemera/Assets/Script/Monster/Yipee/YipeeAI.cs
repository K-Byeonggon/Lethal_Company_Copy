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
        //���� �������� children Node
        ActionNode dead = new ActionNode(Dead);

        //���� �������� children Node��
        ActionNode attackWill = new ActionNode(AttackWill);
        ActionNode moveToPlayer = new ActionNode(MoveToPlayer);
        ActionNode attackPlayer = new ActionNode(AttackPlayer);

        //��ȸ �������� children Node��
        ActionNode setDest = new ActionNode(SetDest);
        ActionNode moveToDest = new ActionNode(MoveToDest);

        //Ž�� �������� children Node��
        ActionNode setDestToScrap = new ActionNode(SetDestToScrap);
        ActionNode moveToScrap = new ActionNode(MoveToScrap);
        ActionNode getScrap = new ActionNode(GetScrap);
        ActionNode moveToNest = new ActionNode(MoveToNest);
        ActionNode setScrap = new ActionNode(SetScrap);

        //���� �������� children Node
        ActionNode threathen = new ActionNode(Threaten);

        //������ ��忡 �� ������ ����
        SequenceNode attackSequence = new SequenceNode(new List<Node> { attackWill, moveToPlayer, attackPlayer });
        SequenceNode wanderSequence = new SequenceNode(new List<Node> { setDest, moveToDest });
        SequenceNode detectSequence = new SequenceNode(new List<Node> { setDestToScrap, moveToScrap, getScrap, moveToNest, setScrap });
        topNode = new SelectorNode(new List<Node> { dead, attackSequence, threathen, wanderSequence, detectSequence, });
    }

    //���� ������ ���
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

    //�������� Ȱ��ȭ(���� ������)
    private Node.State AttackWill()
    {
        //1. �÷��̾�� ���� �޾Ҵ��� bool���� Ȯ��(YipeeHealth����)
        if (isAttacked) 
        { 
            //YipeeHealth���� �÷��̾� ����. 
            return Node.State.SUCCESS; 
        }
        //2. �÷��̾ �ڽ��� 7�� �̻� �Ĵٺô��� Ȯ��
        else if (false) { /*�÷��̾� Transform ����*/ return Node.State.SUCCESS; }
        //3. �÷��̾ ������ ��ǰ ���İ����� ��.
        else if(false) { /*�÷��̾� Transform ����*/ return Node.State.SUCCESS; }
        else return Node.State.FAILURE;
    }

    //�÷��̾�� ����(���� ������)
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

    //�÷��̾ ����(���� ������)
    private Node.State AttackPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            // ���� ���� ����
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

    //���� ������ ���
    private Node.State Threaten()
    {
        if (bewareOf == null || itemHave) return Node.State.FAILURE;
        if (Vector3.Distance(bewareOf.position, nest.position) < 2f)
        {
            if(Vector3.Distance(transform.position, bewareOf.position) < 2f)
            {
                navMeshAgent.SetDestination(transform.position);
                transform.LookAt(bewareOf.position);
                Debug.Log("�����ϴ� ����");
                return Node.State.SUCCESS;
            }
            else return Node.State.RUNNING;
        }
        else return Node.State.RUNNING;
    }

    //���� ������ ����(�ѹ��� ����)(���ƴٴϱ� ������)
    private Node.State SetDest()
    {
        Debug.Log("SetDest");
        if (itemFind) return Node.State.FAILURE;        //�������� ã�Ƽ� �������� �����ϴ� ���¸�,
        else if (setDesti) return Node.State.SUCCESS;   //�̹� ������ ������ �Ǿ�������,
        else
        {
            Vector3 newPos = RandomNavMeshMovement.RandomNavSphere(nest.position, wanderRadius, -1);
            navMeshAgent.SetDestination(newPos);
            setDesti = true;
            return Node.State.SUCCESS;
        }
    }


    //�������� �̵�(���ƴٴϱ� ������)
    private Node.State MoveToDest()
    {
        Debug.Log("MoveToDest");

        if (itemFind) return Node.State.FAILURE;

        //�������� ������.
        else if (Vector3.Distance(transform.position, navMeshAgent.destination) <= 1f)
        {
            setDesti = false;
            return Node.State.SUCCESS;
        }
        else return Node.State.RUNNING;
    }

    //��ǰ���� ������ ����(Ž�� ������)
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

    //��ǰ���� �̵�(Ž�� ������)
    private Node.State MoveToScrap()
    {
        Debug.Log("MoveToScrap");
        if (itemFind)
        {
            //���߿� �������� ���� �ƴ� ���������� �ֿ����� FAILURE.
            if (detectedItem.transform.parent != null && detectedItem.transform.parent != transform.GetChild(1)) return Node.State.FAILURE;

            if (Vector3.Distance(transform.position, navMeshAgent.destination) <= 0.5f)
            {
                return Node.State.SUCCESS;
            }
            else
            {
                Debug.Log("RUNNING�� �ǹ̰� �ֳ�?");
                return Node.State.RUNNING;
            }
        }
        else return Node.State.FAILURE;
    }


    //��ǰ ����(Ž�� ������)
    private Node.State GetScrap()
    {
        Debug.Log("GetScrap");

        if (!itemHave)
        {
            //��ǰ ���

            //���߿� �������� ���� �ƴ� ���������� �ֿ����� FAILURE.
            if (detectedItem.transform.parent != null && detectedItem.transform.parent != transform) return Node.State.FAILURE;

            Debug.Log("���ø�");
            detectedItem.transform.position = transform.GetChild(1).position;
            detectedItem.transform.SetParent(transform.GetChild(1));
            itemHave = true;

            //������ ������ ����.
            navMeshAgent.SetDestination(nest.position);
            setDesti = true;
            return Node.State.SUCCESS;
        }
        else if (itemHave) return Node.State.SUCCESS;
        else return Node.State.FAILURE;

    }

    //��ǰ ��� ������(Ž�� ������)
    private Node.State MoveToNest()
    {
        Debug.Log("MoveToNest");
        if (Vector3.Distance(transform.position, navMeshAgent.destination) <= 0.5f)
        {
            return Node.State.SUCCESS;
        }
        else { return Node.State.RUNNING; }
    }

    //��ǰ ��������(Ž�� ������)
    private Node.State SetScrap()
    {
        if(itemHave)
        {
            Debug.Log("��������");
            detectedItem.transform.parent = null;
            setDesti = false;
            itemFind = false;
            itemHave = false;
            return Node.State.FAILURE;
        }
        else return Node.State.SUCCESS;
    }
}