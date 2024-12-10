using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThumperView : FieldOfView
{
    ThumperAI thumper;

    private List<GameObject> previouslyVisibleTargets = new List<GameObject>();


    private void OnEnable()
    {
        thumper = transform.parent.GetComponent<ThumperAI>();
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
            //덤퍼가 플레이어 봄.
            Debug.Log("덤퍼가 " + targetTransform.name + " 보고 있음.");
            thumper.target = targetTransform;
            thumper.isWillingToAttack = true;
        }
    }
}
