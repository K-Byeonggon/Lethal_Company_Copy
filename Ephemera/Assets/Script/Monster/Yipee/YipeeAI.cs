using Mirror;
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
    [SerializeField] public UnityEngine.AI.NavMeshAgent navMeshAgent;

    [SerializeField] float attackDistance = 1.5f;
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
    [SerializeField] private YipeeHealth yipeeHealth;
    [SerializeField] float attackCooltime = 2f;
    private float lastAttackTime;

    private bool startBeWatched = false;
    private float lastBeWatchedTime;
    public bool sawPlayer = false;
    public List<GameObject> item = new List<GameObject>();

    public override void OnStartServer()
    {
        enabled = true;
        navMeshAgent.enabled = true;
        MonsterReference.Instance.AddMonsterToList(gameObject);
        
        ConstructBehaviorTree();
        damageMessage = new DamageMessage();
        damageMessage.damage = 30;
        damageMessage.damager = gameObject;
    }
    void Update()
    {
        if(isServer)
        {
            if(nest == null)
            {
                GameObject nestObject = new GameObject("Nest");
                nestObject.transform.position = transform.position;
                nest = nestObject.transform;
            }
            CheckWatched(); StolenItem();
            topNode.Evaluate();
        }
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
            OnServerSetItemParent(detectedItem.GetComponent<NetworkIdentity>(), false);
            //detectedItem.transform.parent = null;
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
        //�̰Ŵ� Update���� CheckWatched�� ���� isAttacked�� �ٲ��ش�. �׷��� 1�� ��쿡 �ɷ��� ������.
        
        //3. �÷��̾ ������ ��ǰ ���İ����� ��.
        //�̰ŵ� Update���� StolenItem���� isAttacked�� �ٲ���.

        else return Node.State.FAILURE;
    }

    //�÷��̾�� ����(���� ������)
    private Node.State MoveToPlayer()
    {
        Debug.Log("������ �ϴ� �� ������");
        float distance = Vector3.Distance(transform.position, player.position);
        Debug.Log(player.position + "," + transform.position + ", " + distance);
        if (distance > attackDistance)
        {
            if(detectedItem != null && detectedItem.transform.parent == transform) OnServerSetItemParent(detectedItem.GetComponent<NetworkIdentity>(), false); //detectedItem.transform.parent = null;

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
        Debug.Log("������ ���ϳ�?");
        if (Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            // ���� ���� ����
            if (Time.time - lastAttackTime >= attackCooltime)
            {
                LivingEntity playerHealth = player.GetComponent<LivingEntity>();
                playerHealth.ApplyDamage(damageMessage);
                lastAttackTime = Time.time;
                if (playerHealth.dead) { isAttacked = false; player = null; }

                return Node.State.FAILURE;
            }
        }
        return Node.State.SUCCESS;
    }

    //���� ������ ���
    private Node.State Threaten()
    {
        if (bewareOf == null || itemHave) return Node.State.FAILURE;
        if (Vector3.Distance(bewareOf.position, nest.position) < 3f)
        {
            if(Vector3.Distance(transform.position, bewareOf.position) < 3f)
            {
                navMeshAgent.SetDestination(transform.position);
                Vector3 lookPosition = new Vector3(bewareOf.position.x, transform.position.y, bewareOf.position.z);
                transform.LookAt(lookPosition);
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
            //detectedItem.transform.position = transform.GetChild(1).position;
            //detectedItem.transform.SetParent(transform.GetChild(1));
            OnServerSetItemParent(detectedItem.GetComponent<NetworkIdentity>(), true);
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
            //detectedItem.transform.parent = null;
            OnServerSetItemParent(detectedItem.GetComponent<NetworkIdentity>(), false);
            setDesti = false;
            itemFind = false;
            itemHave = false;
            return Node.State.FAILURE;
        }
        else return Node.State.SUCCESS;
    }

    private void CheckWatched()
    {
        if (!startBeWatched && beWatched)
        {
            startBeWatched = true;
            lastBeWatchedTime = Time.time;
        }

        if(!beWatched)
        {
            Debug.Log("�Ⱥ��� ����");
            startBeWatched = false;
        }

        if (beWatched && Time.time - lastBeWatchedTime > 7f)
        {
            Debug.Log("7�� �Ĵ� ����");
            isAttacked = true;
            player = watchedBy.transform;
        }
    }

    private void StolenItem()
    {
        if (sawPlayer)
        {
            if(Vector3.Distance(transform.position, nest.position) < 4f)
            {
                foreach(var i in item)
                {
                    if(i.transform.parent != null)
                    {
                        isAttacked = true;
                        break;
                    }
                }
            }
        }
    }
    [Server]
    public void OnServerSetItemParent(NetworkIdentity itemIdentity, bool isParent)
    {
        OnClientSetItemParent(itemIdentity, isParent);
    }
    [ClientRpc]
    public void OnClientSetItemParent(NetworkIdentity itemIdentity, bool isParent)
    {
        var itemComponent = itemIdentity.GetComponent<Item>();

        itemComponent.itemCollider.enabled = !isParent;
        itemComponent.rigid.isKinematic = isParent;
        itemComponent.rigid.useGravity = !isParent;
        itemComponent.rigid.velocity = Vector3.zero;
        itemComponent.rigid.angularVelocity = Vector3.zero;

        if(isParent == true)
        {
            itemComponent.transform.parent = transform.GetChild(1);
            itemComponent.transform.position = transform.GetChild(1).position;
        }
        else
        {
            itemComponent.transform.parent = null;
        }
    }
}