using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SnareFleaAI : MonsterAI
{

    private Node topNode;
    [SerializeField] public UnityEngine.AI.NavMeshAgent navMeshAgent;
    [SerializeField] SnareFleaHealth snareHealth;
    private DamageMessage damageMessage;
    [SerializeField] float attackCooltime = 2f;
    private float lastAttackTime;
    public Transform player;

    public bool sawPlayer = false;
    public bool atCeiling = false;
    public bool isGrounded;
    public float checkDistance = 1.0f; // ���� üũ�� �ִ� �Ÿ�
    public LayerMask groundLayer; // �� ���̾� ����
    public LayerMask ceilingLayer; // õ�� ���̾� ����
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] float bindDistance = 1f;
    [SerializeField] float attackDistance = 1.5f;
    [SerializeField] float runDistance = 8f;
    [SerializeField] float jumpForce = 100f;
    public bool isAttacked = false;
    public bool isRunning = false;
    public bool jumped = false;
    float jumpedTime = 0;


    void Start()
    {
        openDoorDelay = 4f;
        ConstructBehaviorTree();

        damageMessage = new DamageMessage();
        damageMessage.damage = 10;
        damageMessage.damager = gameObject;
    }
    public override void OnStartServer()
    {
        enabled = true;
        navMeshAgent.enabled = true;
        MonsterReference.Instance.AddMonsterToList(gameObject);
    }
    void Update()
    {
        if (isServer)
        {
            CheckGround();
            //CheckCeiling();
            topNode.Evaluate();
        }
    }

    private void ConstructBehaviorTree()
    {
        //���� �������� children Node
        ActionNode dead = new ActionNode(Dead);

        //���� ������
        ActionNode spawned = new ActionNode(Spawned);

        //Ž�� ������
        ActionNode detect = new ActionNode(Detect);

        //���� ������
        ActionNode bind = new ActionNode(Bind);
        ActionNode attackPlayer = new ActionNode(AttackPlayer);

        //���� ���� ������
        ActionNode moveToPlayer = new ActionNode(MoveToPlayer);
        ActionNode bindFromGround = new ActionNode(BindFromGround);
        //ActionNode attackPlayer

        //���� ������
        ActionNode runFromPlayer = new ActionNode(RunFromPlayer);
        ActionNode toCeiling = new ActionNode(ToCeiling);
        ActionNode hangOn = new ActionNode(HangOn);

        SequenceNode attackSequence = new SequenceNode(new List<Node> { bind, attackPlayer });
        SequenceNode groundAttackSequence = new SequenceNode(new List<Node> { moveToPlayer, bindFromGround, attackPlayer });
        SequenceNode runSequence = new SequenceNode(new List<Node> { runFromPlayer, toCeiling, hangOn });


        topNode = new SelectorNode(new List<Node> { dead, spawned, detect, attackSequence, groundAttackSequence, runSequence });
        
    }

    private Node.State Dead()
    {
        if (snareHealth.IsDead)
        {
            navMeshAgent.enabled = false;
            return Node.State.SUCCESS;
        }
        else return Node.State.FAILURE;
    }
    
    private Node.State Spawned()
    {
        if(!sawPlayer && isGrounded && !jumped)
        {
            Debug.Log("����");
            jumped = true;
            navMeshAgent.enabled = false;
            rigidbody.useGravity = false;
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpedTime = Time.time;
        }
        return Node.State.FAILURE;
    }

    //[Ž�� ������] �÷��̾� Ž���ؼ� ��������
    private Node.State Detect()
    {
        if (sawPlayer)
        {
            rigidbody.useGravity = true;
            return Node.State.FAILURE;
        }
        else
        {
            return Node.State.SUCCESS;
        }
    }


    //[���� ������] �÷��̾ ������ �޶�ٱ�. �÷��̾ ���� ��� �������� ����߷����Ѵ�.
    private Node.State Bind()
    {
        //player.GetChild(0): �÷��̾��� �Ӹ��κ��� �� ������Ʈ
        if(Vector3.Distance(transform.position, player.GetChild(0).position) <= bindDistance)
        {
            Debug.Log("�޶����");
            transform.position = player.GetChild(0).position;
            rigidbody.useGravity = false;
            return Node.State.SUCCESS;
        }
        else
        {
            if (isGrounded)
            {
                Debug.Log("�޶�ٱ� ����");
                return Node.State.FAILURE;
            }
            else
            {
                Debug.Log("�������� ��");
                return Node.State.RUNNING;
            }
        }
    }

    //[���� ������, ���� ���� ������] �޶�پ� ������, �����ð����� �������� �ش�. �÷��̾� ������ ��������.
    private Node.State AttackPlayer()
    {
        //���� ������ ���� ��������(���� -> ������� -> ����, �������->����)
        if (isAttacked) { rigidbody.useGravity = true; return Node.State.FAILURE; }

        if (Vector3.Distance(transform.position, player.GetChild(0).position) <= bindDistance)
        {   
            if (Time.time - lastAttackTime >= attackCooltime)
            {
                Debug.Log("�����Ѵ�.");
                LivingEntity playerHealth = player.GetComponent<LivingEntity>();

                //�÷��̾� �׾����� ���� ��������
                if (playerHealth.IsDead) { rigidbody.useGravity = true; return Node.State.FAILURE; }

                //�ƴϸ� ������ ����.
                playerHealth.ApplyDamage(damageMessage);
                lastAttackTime = Time.time;

                return Node.State.SUCCESS;
            }
            else return Node.State.RUNNING;
        }
        else { return Node.State.FAILURE; }
    }

    //[���� ���� ������] �÷��̾ ���� �ٰ���
    private Node.State MoveToPlayer()
    {
        //���ݴ��ϸ�/�÷��̾� �׾����� ���� �������� ���� ��������
        if (isAttacked || player.GetComponent<LivingEntity>().IsDead) { return Node.State.FAILURE; }

        //����޽� ���ش�.
        navMeshAgent.enabled = true;
        navMeshAgent.SetDestination(player.position);

        if (Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            return Node.State.SUCCESS;
        }
        else return Node.State.RUNNING;
    }

    //[���� ���� ������] ���󿡼� �÷��̾ �޶�ٱ�
    private Node.State BindFromGround()
    {
        //���� ������ �÷��̾� ��ü���� �Ÿ��� ���
        if (Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            Debug.Log("�޶����");
            navMeshAgent.enabled = false;
            transform.position = player.GetChild(0).position;
            rigidbody.useGravity = false;
            return Node.State.SUCCESS;
        }
        else
        {
            Debug.Log("�޶�ٱ� ����");
            return Node.State.FAILURE;
        }
    }

    //[���� ������] �÷��̾�� ���� �־���
    private Node.State RunFromPlayer()
    {
        Debug.Log("���� ������");


            if (!isRunning)
            {
                navMeshAgent.enabled = true;
                Vector3 newPos = RandomNavMeshMovement.RandomNavSphere(transform.position, runDistance, -1);
                Debug.Log("������ ����: " + newPos);
                navMeshAgent.SetDestination(newPos);
                isRunning = true;
            }
            return Node.State.SUCCESS;
        
        //else { return Node.State.FAILURE; }
    }

    //[���� ������] ������������ ���������� ����.
    private Node.State ToCeiling()
    {
        if (Vector3.Distance(navMeshAgent.destination, transform.position) < 0.5f)
        {
            Debug.Log("������ ����");
            if (isGrounded && !jumped)
            {
                Debug.Log("����");
                jumped = true;
                navMeshAgent.enabled = false;
                rigidbody.useGravity = false;
                rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                jumpedTime = Time.time;
            }
        }
        else
        {
            return Node.State.RUNNING;
        }
        return Node.State.SUCCESS;
    }
    
    //[���� ������] õ�忡 �ٱ�. ��Ÿ bool �ʱ�ȭ.
    private Node.State HangOn()
    {
        Debug.Log("õ�� �ٱ�!!" + atCeiling);
        //Debug.Log(transform.position.y);
        Debug.Log("�ð�: " + Time.time + " ����: " + jumpedTime);
        if (atCeiling)
        {
            Debug.Log("õ�� ����");
            isAttacked = false;
            rigidbody.useGravity = false;
            sawPlayer = false;
            player = null;
            isRunning = false;
            jumped = false;
            return Node.State.SUCCESS;
        }
        else
        {
            Debug.Log("õ�� ������ �ȵ�~");
            return Node.State.RUNNING;
        }
    }


    public void SetDefault()
    {
        isAttacked = false;
        rigidbody.useGravity = false;
        sawPlayer = false;
        player = null;
        isRunning = false;
        jumped = false;
    }

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

    bool CheckCeiling2()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit, checkDistance, ceilingLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * checkDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * checkDistance);
    }
}