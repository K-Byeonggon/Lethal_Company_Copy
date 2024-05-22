using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ThumperAI : MonsterAI
{
    private Node topNode;
    public Transform player;
    public bool sawPlayer = false;

    public UnityEngine.AI.NavMeshAgent navMeshAgent;

    [SerializeField] float attackDistance = 0.5f;
    public bool setDesti = false;
    public float wanderRadius = 10f;
    public bool isAttacked = false;

    private DamageMessage damageMessage;
    private ThumperHealth thumperHealth;
    [SerializeField] float attackCooltime = 0.33f;
    private float lastAttackTime;


    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        ConstructBehaviorTree();

        thumperHealth = GetComponent<ThumperHealth>();
        damageMessage = new DamageMessage();
        damageMessage.damage = 40;
        damageMessage.damager = gameObject;
    }

    void Update()
    {
        topNode.Evaluate();
    }

    private void ConstructBehaviorTree()
    {
        //���� �������� children Node
        ActionNode dead = new ActionNode(Dead);

        /*
        //���� �������� children Node��
        ActionNode attackWill = new ActionNode(AttackWill);
        ActionNode moveToPlayer = new ActionNode(MoveToPlayer);
        ActionNode attackPlayer = new ActionNode(AttackPlayer);

        //��ȸ �������� children Node��
        ActionNode setDest = new ActionNode(SetDest);
        ActionNode moveToDest = new ActionNode(MoveToDest);
        */

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

    //[���� ������] ���� ���� 1. �÷��̾� �߰�
}