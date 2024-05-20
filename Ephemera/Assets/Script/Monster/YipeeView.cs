using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YipeeView : FieldOfView
{
    public override void FindVisibleTargets()
    {
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
                    //비축벌레가 플레이어를 보고 있을 때 둥지 근처의 수집아이템이 부모가 생기면? 플레이어 공격.
                    Debug.Log("비축벌레가 " + targetTransform.name + " 보고 있음.");

                }
            }
        }
    }
}
