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
                    //플레이어 죽었으면 무시
                    if(!targetTransform.TryGetComponent<LivingEntity>(out LivingEntity player) || player.IsDead)
                    {
                        continue;
                    }
                    //덤퍼가 플레이어 봄.
                    Debug.Log("덤퍼가 " + targetTransform.name + " 보고 있음.");
                    thumper.target = targetTransform;
                    thumper.isWillingToAttack = true;
                }
            }
        }
    }
}
