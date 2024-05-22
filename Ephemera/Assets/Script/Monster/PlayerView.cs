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

        // ��ü�� �����ؼ�, �þ� ���� ��ŭ�� ��¥ ����
        foreach (Collider target in targetsInViewRadius)
        {
            Transform targetTransform = target.transform;
            Vector3 directionToTarget = (targetTransform.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    // ��¥ �þ߰��� �ִ� ����
                    Debug.Log("�÷��̾ " + targetTransform.name + " ���� ����.");
                    MonsterAI targetAI = targetTransform.GetComponent<MonsterAI>();
                    if (targetAI != null)
                    {
                        targetAI.BeWatched = true;
                        targetAI.WatchedBy = transform.parent.gameObject; // �þ� ������Ʈ�� �÷��̾��� ������ ���� ����
                        currentlyVisibleTargets.Add(targetAI);
                    }
                }
            }
        }

        // ������ ������ Ÿ�� �� ���� �������� ���� Ÿ�� ó��
        foreach (MonsterAI targetAI in previouslyVisibleTargets)
        {
            if (!currentlyVisibleTargets.Contains(targetAI))
            {
                targetAI.BeWatched = false;
                targetAI.WatchedBy = null;
            }
        }

        // ���� ������ Ÿ�� ����� ���� ������ Ÿ�� ������� ����
        previouslyVisibleTargets = currentlyVisibleTargets;
    }

}
