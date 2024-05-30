using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SporeLizardAI : MonsterAI
{
    private Node topNode;
    [SerializeField] public UnityEngine.AI.NavMeshAgent navMeshAgent;
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
    Vector3 wanderDest;
    public bool setDesti = false;
    public Transform bewareOf;
    [SerializeField] Transform head;
    [SerializeField] Transform pivot;
    public GameObject sporeParticle;
    [SerializeField] int cornerRayCount = 8;
    [SerializeField] float cornerDetectionRadius = 5f;
    [SerializeField] float attackDistance = 1f;
    [SerializeField] bool attackState = false;


    //���ο� ������ AI
    //��ȿ���� ���� ������
    public enum State
    {
        Wander,
        Threaten,
        Explode,
        Run,
        Attack
    }
    public State currentState;
    public bool sawPlayer = false;
    private Vector3[] rayDirections;


    public override void OnStartServer()
    {
        enabled = true;
        navMeshAgent.enabled = true;
        MonsterReference.Instance.AddMonsterToList(gameObject);
        
        openDoorDelay = 1f;
        currentState = State.Wander;
        rayDirections = new Vector3[cornerRayCount];
        ConstructBehaviorTree();

        damageMessage = new DamageMessage() { damage = 20, damager = gameObject };
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
        ActionNode threaten = new ActionNode(Threaten);
        ActionNode explodeSpore = new ActionNode(ExplodeSpore);

        //����
        ActionNode run = new ActionNode(Run);

        //���� �������� children Node��
        ActionNode moveToPlayer = new ActionNode(MoveToPlayer);
        ActionNode attackPlayer = new ActionNode(AttackPlayer);

        //��ȸ �������� children Node��
        ActionNode wander = new ActionNode(Wander);

        //������ ��忡 �� ������ ����
        SequenceNode attackSequence = new SequenceNode(new List<Node> { moveToPlayer, attackPlayer });
        topNode = new SelectorNode(new List<Node> { wander, threaten, explodeSpore, run, attackSequence });
    }

    //[��ȸ ������] ������ ���� �� �̵�
    private Node.State Wander()
    {
        if (currentState != State.Wander) return Node.State.FAILURE;

        if (sawPlayer)
        {
            if (!bewareOf.GetComponent<LivingEntity>().IsDead)
            {
                currentState = State.Threaten;
                setDesti = false;
                return Node.State.FAILURE;
            }
        }

        if (!setDesti)
        {
            Debug.Log("������ ����");
            Vector3 newDest = RandomNavMeshMovement.RandomNavSphere(transform.position, wanderRadius, -1);
            setDesti = true;
            navMeshAgent.SetDestination(newDest);
        }

        if (Vector3.Distance(head.position, navMeshAgent.destination) <= 1f)
        {
            Debug.Log("������ ����");
            setDesti = false;
            return Node.State.SUCCESS;
        }
        else return Node.State.RUNNING;
    }

    //[���� ������] �÷��̾� ����
    private Node.State Threaten()
    {
        //���� ���ð� 3~5�� ���� ����
        //�Ǵ� �÷��̾� ��¥ ������ �ٰ����� ������.
        if (currentState != State.Threaten) return Node.State.FAILURE;

        if (Vector3.Distance(transform.position, bewareOf.position) < threatDistance)
        {
            if (!isThreatening)
            {
                isThreatening = true;
                threatTime = Time.time;
                threatDuration = Random.Range(3f, 5f);
            }

            //�����ϱ�
            if (Time.time - threatTime < threatDuration)
            {
                //rigidbody.velocity = Vector3.zero;
                //rigidbody.isKinematic = true;
                navMeshAgent.isStopped = true;
                Vector3 lookPosition = new Vector3(bewareOf.position.x, 0, bewareOf.position.z);
                pivot.LookAt(lookPosition);
                Debug.Log("���� ��");

                return Node.State.RUNNING;
            }
            else
            {
                //rigidbody.isKinematic = false;
                navMeshAgent.isStopped = false;
                Debug.Log("���� ��");
                isThreatening = false;
                currentState = State.Explode;
                return Node.State.FAILURE;
            }
        }
        else
        {
            //rigidbody.isKinematic = false;
            navMeshAgent.isStopped = false;
            Debug.Log("���� ���� ����");
            currentState = State.Wander;
            isThreatening = false;
            return Node.State.FAILURE;
        }
    }

    //[���� ������] ���� �߻�
    private Node.State ExplodeSpore()
    {
        if (currentState != State.Explode) return Node.State.FAILURE;

        if (Random.value <= sporePercentage)
        {
            if (haveSpore)
            {
                Debug.Log("���� �߻�");
                haveSpore = false;
                //Instantiate(sporeParticle, transform.position, Quaternion.identity);
                OnServerInstantiateParticle();
            }
        }

        //�����̾����� ���ݽ�������
        if (IsInCorner()) { haveSpore = true; currentState = State.Attack; return Node.State.FAILURE; }
        else { haveSpore = true; currentState = State.Run; return Node.State.FAILURE; }
    }
    [Server]
    public void OnServerInstantiateParticle()
    {
        OnClientInstantiateParticle();
    }
    [ClientRpc]
    public void OnClientInstantiateParticle()
    {
        Instantiate(sporeParticle, transform.position, Quaternion.identity);
    }

    //[���� ������] ���������� ���� �� ����
    private Node.State Run()
    {
        if(currentState != State.Run) return Node.State.FAILURE;

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

        if (Vector3.Distance(head.transform.position, wanderDest) <= 1f)
        {
            Debug.Log("���� ������ ����");
            setDesti = false;
            navMeshAgent.speed = defaultSpeed;
            navMeshAgent.acceleration = defaultAccel;
            navMeshAgent.angularSpeed = defaultAngle;
            bewareOf = null;

            currentState = State.Wander;
            return Node.State.SUCCESS;
        }
        else return Node.State.RUNNING;
    }

    //[���� ������] �÷��̾�� ����
    private Node.State MoveToPlayer()
    {
        if (currentState != State.Attack) return Node.State.FAILURE;

        float distance = Vector3.Distance(head.transform.position, bewareOf.position);
        Debug.Log("�����Ϸ� �̵�" + head.transform.position + ", " + bewareOf.position + ", " + Vector3.Distance(head.transform.position, bewareOf.position));
        if (distance > attackDistance)
        {
            Debug.Log("���� �������� ���ߴ�.");
            navMeshAgent.SetDestination(bewareOf.position);
            return Node.State.RUNNING;
        }
        else
        {
            Debug.Log("�����ߴ�");
            return Node.State.SUCCESS;
        }
    }

    //[���� ������] �÷��̾ ����
    private Node.State AttackPlayer()
    {
        Debug.Log("�÷��̾� ���� ���");
        //Debug.Log(head.position + ", " + bewareOf.position + ", " + Vector3.Distance(head.position, bewareOf.position));

        if (Vector3.Distance(head.transform.position, bewareOf.position) <= attackDistance)
        {
            if (Time.time - lastAttackTime >= attackCooltime)
            {
                Debug.Log("���ݼ���");
                LivingEntity playerHealth = bewareOf.GetComponent<LivingEntity>();
                playerHealth.ApplyDamage(damageMessage);
                lastAttackTime = Time.time;

                if (playerHealth.IsDead) currentState = State.Wander;
                return Node.State.SUCCESS;
            }
            else
            {
                return Node.State.RUNNING;
            }
        }
        else
        {
            return Node.State.FAILURE;
        }

    }



    //������ �����ִ��� Ȯ����. (���� -> �������� �Ѿ�� ���� ����)
    bool IsInCorner()
    {
        int blockedRayCount = 0;
        for (int i = 0; i < cornerRayCount; i++)
        {
            float angle = i * (360f / cornerRayCount);
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            rayDirections[i] = direction;  // Ray ���� ����

            NavMeshHit hit;
            if (NavMesh.Raycast(transform.position, transform.position + direction * cornerDetectionRadius, out hit, NavMesh.AllAreas))
            {
                blockedRayCount++;
            }
        }
        return blockedRayCount > cornerRayCount / 2; // ���� �̻��� ������ ���� ������ �������� �Ǵ�
    }

    void OnDrawGizmos()
    {
        if (rayDirections == null) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < rayDirections.Length; i++)
        {
            Gizmos.DrawRay(transform.position, rayDirections[i] * cornerDetectionRadius);
        }
    }
}
