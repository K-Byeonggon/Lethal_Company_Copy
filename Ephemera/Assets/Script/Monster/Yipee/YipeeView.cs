using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YipeeView : FieldOfView
{
    YipeeAI yipee;

    private List<GameObject> previouslyVisibleTargets = new List<GameObject>();

    private void OnEnable()
    {
        yipee = transform.parent.GetComponent<YipeeAI>();
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

                //시야가 벽에 안막혀있으면
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    //비축벌레가 아이템을 발견
                    //아이템은 그냥 콜라이더로 감지하기로함.

                    //비축벌레가 플레이어를 보고 있을 때 둥지 근처의 수집아이템이 부모가 생기면? 플레이어 공격.
                    if(target.gameObject.layer == LayerMask.NameToLayer("Player"))
                    {
                        Debug.Log("비축벌레가 " + targetTransform.name + " 보고 있음.");
                        yipee.sawPlayer = true;
                        yipee.player = targetTransform;
                    }

                    if(target.gameObject.layer == LayerMask.NameToLayer("Item"))
                    {
                        Debug.Log("비축벌레가 " + targetTransform.name + "보고 있음.");
                        yipee.item.Add(target.gameObject);
                    }

                    currentlyVisibleTargets.Add(targetTransform.gameObject);

                }
            }
        }

        // 이전에 감지된 타겟 중 현재 감지되지 않은 타겟 처리
        foreach (GameObject player in previouslyVisibleTargets)
        {
            if (!currentlyVisibleTargets.Contains(player))
            {
                if(player.layer == LayerMask.NameToLayer("Player"))
                {
                    yipee.sawPlayer = false;
                    yipee.player = null;
                }

                if(player.tag == "ObtainableItem")
                {
                    yipee.item.Remove(player);
                }
            }
        }

        // 현재 감지된 타겟 목록을 이전 감지된 타겟 목록으로 갱신
        previouslyVisibleTargets = currentlyVisibleTargets;

    }
}
