using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoilheadView : FieldOfView
{
    CoilheadAI coilhead;

    private void OnEnable()
    {
        coilhead = transform.parent.GetComponent<CoilheadAI>();
    }

    private bool IsTargetVisible(Transform targetTransform)
    {
        Vector3 directionToTarget = (targetTransform.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);

        // 시야 각도 안에 있는지 확인
        if (Vector3.Angle(transform.forward, directionToTarget) >= viewAngle / 2)
            return false;

        // 장애물로 막혀 있는지 확인
        if (Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
            return false;

        // 감지 타겟이 살아있는지 확인
        if (!targetTransform.TryGetComponent(out LivingEntity player) || player.IsDead)
            return false;

        return true;
    }

    public override void FindVisibleTargets()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        //시야 범위 만큼만 감지
        foreach (Collider target in targetsInViewRadius)
        {
            Transform targetTransform = target.transform;

            if (!IsTargetVisible(targetTransform))
                continue;

            float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);

            // 코일 헤드는 시야의 가장 가까운 플레이어를 target으로 한다.
            //이전 target이 없었으면 targetTransform을 target으로
            //targetTransform이 target의 위치보다 가까우면 targetTransform을 target으로
            if (coilhead.target == null || 
                Vector3.Distance(transform.position, coilhead.target.position) > distanceToTarget)
            {
                Debug.Log($"코일헤드가 {target.name}을 새로운 타겟으로");
                coilhead.target = targetTransform;
            }
        }
    }
}
