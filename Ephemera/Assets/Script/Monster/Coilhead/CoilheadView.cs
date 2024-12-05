using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoilheadView : FieldOfView
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

        //��ü�� �����ؼ�, �þ� ���� ��ŭ�� ��¥ ����.
        foreach (Collider target in targetsInViewRadius)
        {
            Transform targetTransform = target.transform;
            Vector3 directionToTarget = (targetTransform.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {                    
                    //�÷��̾� �׾����� ����
                    if (targetTransform.GetComponent<LivingEntity>().IsDead) { continue; }
                    //�������� �÷��̾ �߰��ϸ� ���� �������� �����Ѵ�.
                    Debug.Log("������尡 " + targetTransform.name + " ���� ����.");
                    coilhead.sawPlayer = true;
                    coilhead.target = targetTransform;
                    currentlyVisibleTargets.Add(targetTransform.gameObject);
                }
            }
        }

        // ������ ������ Ÿ�� �� ���� �������� ���� Ÿ�� ó��
        foreach (GameObject player in previouslyVisibleTargets)
        {
            if (!currentlyVisibleTargets.Contains(player))
            {
                coilhead.sawPlayer = false;
                coilhead.target = null;
            }
        }

        // ���� ������ Ÿ�� ����� ���� ������ Ÿ�� ������� ����
        previouslyVisibleTargets = currentlyVisibleTargets;
    }
}
