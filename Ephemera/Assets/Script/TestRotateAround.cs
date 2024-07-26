using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotateAround : MonoBehaviour
{
    public Transform target;  // ȸ���� ù ��° ������Ʈ (�߽��� �� ������Ʈ)
    public Transform objectToRotate;  // ȸ���� �� ��° ������Ʈ
    private Quaternion lastRotation;  // ���� �������� ȸ����

    void Start()
    {
        if (target == null || objectToRotate == null)
        {
            Debug.LogError("Both target and objectToRotate must be assigned.");
            enabled = false;
            return;
        }
        // �ʱ� ȸ���� ����
        lastRotation = target.rotation;
    }

    void Update()
    {
        // ���� ȸ���� ��������
        Quaternion currentRotation = target.rotation;

        // ���� �����Ӱ� ���� �������� ȸ���� ���̸� ���
        Quaternion rotationDelta = currentRotation * Quaternion.Inverse(lastRotation);

        // ������Ʈ�� ȸ��
        objectToRotate.position = RotatePointAroundPivot(objectToRotate.position, target.position, rotationDelta);

        // �� ��° ������Ʈ�� ȸ���� �Բ� ����
        objectToRotate.rotation = rotationDelta * objectToRotate.rotation;

        // ���� ȸ������ ���� ȸ�������� ������Ʈ
        lastRotation = currentRotation;
    }

    // Ư�� ���� �ǹ��� �߽����� ȸ����Ű�� �Լ�
    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
    {
        return rotation * (point - pivot) + pivot;
    }
}
