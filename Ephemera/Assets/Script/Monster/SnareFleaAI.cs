using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SnareFleaAI : MonsterAI
{

    private Node topNode;
    public UnityEngine.AI.NavMeshAgent navMeshAgent;
    SnareFleaHealth snareHealth;
    private DamageMessage damageMessage;
    [SerializeField] float attackCooltime = 2f;
    private float lastAttackTime;
    public Transform player;

    public bool sawPlayer = false;
    [SerializeField] bool atCeiling = false;
    [SerializeField] bool isGrounded;
    public float checkDistance = 1.0f; // ���� üũ�� �ִ� �Ÿ�
    public LayerMask groundLayer; // �� ���̾� ����
    public LayerMask ceilingLayer; // õ�� ���̾� ����
    private Rigidbody rigidbody;
    [SerializeField] float bindDistance = 1f;
    public bool isAttacked = false;
    public bool isRunning = false;

    void Start()
    {
        //navMeshAgent = transform.parent.GetComponent<NavMeshAgent>();
        ConstructBehaviorTree();

        snareHealth = GetComponent<SnareFleaHealth>();
        rigidbody = GetComponent<Rigidbody>();
        damageMessage = new DamageMessage();
        damageMessage.damage = 10;
        damageMessage.damager = gameObject;
    }

    void Update()
    {
        CheckGround();
        CheckCeiling();
        topNode.Evaluate();
    }

    private void ConstructBehaviorTree()
    {
        //���� �������� children Node
        ActionNode dead = new ActionNode(Dead);

        ActionNode detect = new ActionNode(Detect);

        //���� ������
        ActionNode bind = new ActionNode(Bind);
        ActionNode attackPlayer = new ActionNode(AttackPlayer);

        //���� ���� ������
        ActionNode runFromPlayer = new ActionNode(RunFromPlayer);
        ActionNode toCeiling = new ActionNode(ToCeiling);

        SequenceNode attackSequence = new SequenceNode(new List<Node> { bind, attackPlayer });
        SequenceNode runSequence = new SequenceNode(new List<Node> { runFromPlayer, toCeiling });
        topNode = new SelectorNode(new List<Node> { dead, detect, attackSequence, runSequence });
    }

    private Node.State Dead()
    {
        if (snareHealth.IsDead)
        {
            navMeshAgent.SetDestination(transform.position);
            return Node.State.SUCCESS;
        }
        else return Node.State.FAILURE;
    }

    //[Ž�� ������] �÷��̾� Ž���ϱ�
    private Node.State Detect()
    {
        if (sawPlayer)
        {
            rigidbody.useGravity = true;
            return Node.State.FAILURE;
        }
        else return Node.State.SUCCESS;
    }

    //[���� ������] �÷��̾ ������ �޶�ٱ�. �÷��̾ ���� ��� �������� ����߷����Ѵ�.
    private Node.State Bind()
    {
        if(Vector3.Distance(transform.position, player.position) <= bindDistance)
        {
            transform.position = player.GetChild(0).position;
            rigidbody.useGravity = false;
            return Node.State.SUCCESS;
        }
        else
        {
            return Node.State.FAILURE;
        }
    }

    //[���� ������] �޶�پ� ������, �����ð����� �������� �ش�.
    private Node.State AttackPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= bindDistance)
        {
            if (Time.time - lastAttackTime >= attackCooltime)
            {
                Debug.Log("�����Ѵ�.");
                LivingEntity playerHealth = player.GetComponent<LivingEntity>();

                //�÷��̾� �׾����� ��ȸ ��������
                if (playerHealth.IsDead) { rigidbody.useGravity = true; return Node.State.FAILURE; }

                //�ƴϸ� ������ ����.
                playerHealth.ApplyDamage(damageMessage);
                lastAttackTime = Time.time;

                return Node.State.SUCCESS;
            }
        }
        return Node.State.FAILURE;
    }

    //[���� ���� ������] �÷��̾�� ���� �־���
    private Node.State RunFromPlayer()
    {
        if (isAttacked)
        {
            if (!isRunning)
            {
                Vector3 newPos = RandomNavMeshMovement.RandomAwayFromPlayer(transform.position, 2f, -1, player.position);
                navMeshAgent.SetDestination(transform.position);
                isRunning = true;
            }
            return Node.State.SUCCESS;
        }
        else { return Node.State.FAILURE; }
    }

    //[���� ���� ������] ������������ ���������� õ�忡 ����
    
    private Node.State ToCeiling()
    {
        if (isGrounded)
        {
            rigidbody.AddForce(Vector3.up * 100f);
        }
        return Node.State.SUCCESS;
    }

    //[���� ���� ������] �÷��̾ ���� �ٰ���
    /*
    private Node.State MoveToPlayer()
    {
        if(Vector2.Distance(transform.position, player.position) <= bindDistance)
        {
            navMeshAgent.SetDestination(player.position);

        }
    }*/


    //[õ�� ������] ���� �ְ�, ��ǥ �÷��̾ ������ õ�忡 �ٴ´�.


    void CheckGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, checkDistance, groundLayer))
        {
            isGrounded = true;
            Debug.Log("Grounded");
        }
        else
        {
            isGrounded = false;
            Debug.Log("Not Grounded");
        }
    }

    void CheckCeiling()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit, checkDistance, ceilingLayer))
        {
            atCeiling = true;
            Debug.Log("atCeiling");
        }
        else
        {
            atCeiling = false;
            Debug.Log("Not at Ceiling");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * checkDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * checkDistance);
    }
}
