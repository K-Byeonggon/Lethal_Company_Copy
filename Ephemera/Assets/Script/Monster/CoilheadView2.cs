using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoilheadView2 : FieldOfView1
{
    CoilheadAI coilhead;

    private List<GameObject> previouslyVisibleTargets = new List<GameObject>();
    

    private void OnEnable()
    {
        coilhead = transform.parent.GetComponent<CoilheadAI>();
    }


    public override void FindVisibleTargets()
    {
        List<GameObject> currentlyVisibleTargets = new List<GameObject>();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        //구체로 감지해서, 시야 각도 만큼만 진짜 감지.
        foreach (Collider target in targetsInViewRadius)
        {
            Transform targetTransform = target.transform;
            Vector3 directionToTarget = (targetTransform.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    //코일헤드는 플레이어를 발견하면 공격 시퀀스를 수행한다.
                    Debug.Log("코일헤드가 " + targetTransform.name + " 보고 있음.");
                    coilhead.sawPlayer = true;
                    coilhead.target = targetTransform;
                    currentlyVisibleTargets.Add(targetTransform.gameObject);
                }
            }
        }

        // 이전에 감지된 타겟 중 현재 감지되지 않은 타겟 처리
        foreach (GameObject player in previouslyVisibleTargets)
        {
            if (!currentlyVisibleTargets.Contains(player))
            {
                coilhead.sawPlayer = false;
                coilhead.target = null;
            }
        }

        // 현재 감지된 타겟 목록을 이전 감지된 타겟 목록으로 갱신
        previouslyVisibleTargets = currentlyVisibleTargets;
    }
}
