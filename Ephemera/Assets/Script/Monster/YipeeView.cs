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

        //��ü�� �����ؼ�, �þ� ���� ��ŭ�� ��¥ ����.
        foreach (Collider target in targetsInViewRadius)
        {
            Transform targetTransform = target.transform;
            Vector3 directionToTarget = (targetTransform.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);

                //�þ߰� ���� �ȸ���������
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    //��������� �������� �߰�

                    //��������� �÷��̾ ���� ���� �� ���� ��ó�� ������������ �θ� �����? �÷��̾� ����.
                    Debug.Log("��������� " + targetTransform.name + " ���� ����.");

                    currentlyVisibleTargets.Add(targetTransform.gameObject);

                }
            }
        }

        // ������ ������ Ÿ�� �� ���� �������� ���� Ÿ�� ó��
        foreach (GameObject player in previouslyVisibleTargets)
        {
            if (!currentlyVisibleTargets.Contains(player))
            {

            }
        }

        // ���� ������ Ÿ�� ����� ���� ������ Ÿ�� ������� ����
        previouslyVisibleTargets = currentlyVisibleTargets;

    }
}