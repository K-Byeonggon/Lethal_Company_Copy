using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : FieldOfView
{
    private List<MonsterAI> previouslyVisibleTargets = new List<MonsterAI>();

    public override void FindVisibleTargets()
    {
        List<MonsterAI> currentlyVisibleTargets = new List<MonsterAI>();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        // 구체로 감지해서, 시야 각도 만큼만 진짜 감지
        foreach (Collider target in targetsInViewRadius)
        {
            Transform targetTransform = target.transform;
            Vector3 directionToTarget = (targetTransform.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    // 진짜 시야각에 있는 몬스터
                    //Debug.Log("플레이어가 " + targetTransform.name + " 보고 있음.");
                    MonsterAI targetAI = targetTransform.GetComponent<MonsterAI>();
                    if (targetAI != null)
                    {
                        targetAI.BeWatched = true;
                        targetAI.WatchedBy = transform.parent.gameObject; // 시야 오브젝트를 플레이어의 하위에 넣을 예정
                        currentlyVisibleTargets.Add(targetAI);
                    }
                }
            }
        }

        // 이전에 감지된 타겟 중 현재 감지되지 않은 타겟 처리
        foreach (MonsterAI targetAI in previouslyVisibleTargets)
        {
            if (!currentlyVisibleTargets.Contains(targetAI))
            {
                targetAI.BeWatched = false;
                targetAI.WatchedBy = null;
            }
        }

        // 현재 감지된 타겟 목록을 이전 감지된 타겟 목록으로 갱신
        previouslyVisibleTargets = currentlyVisibleTargets;
    }

}
