using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CoilheadAI : MonoBehaviour
{
    private Node topNode;
    public UnityEngine.AI.NavMeshAgent navMeshAgent;
    private DamageMessage damageMessage;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        //ConstructBehaviorTree();

        damageMessage = new DamageMessage();
        damageMessage.damage = 100;
        damageMessage.damager = gameObject;
    }
    /*
    private void ConstructBehaviorTree()
    {
        //코일헤드는 처치 불가능 몬스터. 체력도 죽음도 없음.

        //정지 시퀀스
        ActionNode stop = new ActionNode(Stop);

        //공격 시퀀스의 children Node들
        ActionNode attackWill = new ActionNode(AttackWill);
        ActionNode moveToPlayer = new ActionNode(MoveToPlayer);
        ActionNode attackPlayer = new ActionNode(AttackPlayer);

        //배회 시퀀스의 children Node들
        ActionNode setDest = new ActionNode(SetDest);
        ActionNode moveToDest = new ActionNode(MoveToDest);

    }

    void Update()
    {
        topNode.Evaluate();
    }

    //[정지 시퀀스] 정지
    private Node.State Stop()
    {
        //플레이어가 보고 있으면 SUCCESS로 정지.
        //아니면 FAILURE
    }

    //[공격 시퀀스] 공격 의지
    private Node.State AttackWill()
    {
        //플레이어를 발견했으면 속도가 빨라지고 대상 플레이어를 목표로 지정.
    }

    //[공격 시퀀스] 플레이어를 향해 이동
    private Node.State MoveToPlayer()
    {

    }

    //[공격 시퀀스] 플레이어 공격
    private Node.State AttackPlayer()
    {

    }
    */
}
