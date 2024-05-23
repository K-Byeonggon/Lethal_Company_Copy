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
        //���� �������� children Node��

        //���� �������� children Node

        //���� �������� children Node
        ActionNode threaten = new ActionNode(Threaten);
        ActionNode explodeSpore = new ActionNode(ExplodeSpore);
        ActionNode setRunDest = new ActionNode(SetRunDest);
        ActionNode runFromPlayer = new ActionNode(RunFromPlayer);

        //��ȸ �������� children Node��
        ActionNode setDest = new ActionNode(SetDest);
        ActionNode moveToDest = new ActionNode(MoveToDest);

        //������ ��忡 �� ������ ����
        SequenceNode threatSequence = new SequenceNode(new List<Node> { threaten, explodeSpore, setRunDest, runFromPlayer });
        SequenceNode wanderSequence = new SequenceNode(new List<Node> { setDest, moveToDest });
        topNode = new SelectorNode(new List<Node> { threatSequence, wanderSequence });
    }


    //[���� ������] �÷��̾� ����
    private Node.State Threaten()
    {
        //���� ���ð� 3~5�� ���� ����
        //�Ǵ� �÷��̾� ��¥ ������ �ٰ����� ������.
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

            //�����ϱ�
            if (Time.time - threatTime < threatDuration)
            {
                //�÷��̾ �� �����̿��� ����
                if (Vector3.Distance(transform.position, bewareOf.position) < runDistance) return Node.State.SUCCESS;

                navMeshAgent.SetDestination(transform.position);
                pivot.LookAt(bewareOf.position);
                Debug.Log("���� ��");
                
                return Node.State.RUNNING;
            }
            else
            {
                Debug.Log("���� ��");
                return Node.State.SUCCESS;
            }
        }
        else return Node.State.FAILURE;
    }
    
    //[���� ������] ���� �߻� & ������
    private Node.State ExplodeSpore()
    {
        //�̰� ���� ���� ���� �����ɰ� ����.
        //���� �߻� ���θ� Ȯ���� ����
        if (Random.value <= sporePercentage) 
        {
            if(haveSpore)
            {
                Debug.Log("���� �߻�");
                haveSpore = false;
                Instantiate(sporeParticle, transform.position, Quaternion.identity);
            }
        }
        return Node.State.SUCCESS;
    }

    //[���� ������] ���������� ����
    private Node.State SetRunDest()
    {
        if (!setDesti)
        {
            Debug.Log("���� ������ ����");
            wanderDest = RandomNavMeshMovement.NavAwayFromPlayer(transform.position, bewareOf.position, runRadius);
            navMeshAgent.SetDestination(wanderDest);
            setDesti = true;
            navMeshAgent.speed = runSpeed;
            navMeshAgent.acceleration = runAccel;
            navMeshAgent.angularSpeed = runAngle;
        }
        return Node.State.SUCCESS;
    }

    //[���� ������] ����
    private Node.State RunFromPlayer() 
    {
        if (Vector3.Distance(head.position, wanderDest) <= 1f)
        {
            Debug.Log("���� ������ ����");
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

    //[��ȸ ������] ������ ����
    private Node.State SetDest()
    {
        if (!setDesti)
        {
            Debug.Log("������ ����");
            wanderDest = RandomNavMeshMovement.RandomNavSphere(transform.position, wanderRadius, -1);
            navMeshAgent.SetDestination(wanderDest);
            setDesti = true;
        }
        return Node.State.SUCCESS;
    }

    //[��ȸ ������] �̵�
    private Node.State MoveToDest()
    {
        if (Vector3.Distance(head.position, wanderDest) <= 1f)
        {
            Debug.Log("������ ����");
            setDesti = false;
            return Node.State.SUCCESS;
        }
        else return Node.State.RUNNING;
    }

}
