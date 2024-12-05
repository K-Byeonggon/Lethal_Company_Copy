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
                    if(targetTransform.GetComponent<LivingEntity>().IsDead) { continue; }
                    //���۰� �÷��̾� ��.
                    Debug.Log("���۰� " + targetTransform.name + " ���� ����.");
                    thumper.sawPlayer = true;
                    thumper.destination = targetTransform.position;
                    thumper.target = targetTransform;
                    thumper.setDesti = true;
                    currentlyVisibleTargets.Add(targetTransform.gameObject);
                }
            }
        }

        // ������ ������ Ÿ�� �� ���� �������� ���� Ÿ�� ó��
        foreach (GameObject player in previouslyVisibleTargets)
        {
            if (!currentlyVisibleTargets.Contains(player))
            {
                thumper.sawPlayer = false;
            }
        }

        // ���� ������ Ÿ�� ����� ���� ������ Ÿ�� ������� ����
        previouslyVisibleTargets = currentlyVisibleTargets;
    }
}
