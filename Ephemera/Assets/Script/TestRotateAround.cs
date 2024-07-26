using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotateAround : MonoBehaviour
{
    public Transform target;  // 회전할 첫 번째 오브젝트 (중심이 될 오브젝트)
    public Transform objectToRotate;  // 회전할 두 번째 오브젝트
    private Quaternion lastRotation;  // 이전 프레임의 회전값

    void Start()
    {
        if (target == null || objectToRotate == null)
        {
            Debug.LogError("Both target and objectToRotate must be assigned.");
            enabled = false;
            return;
        }
        // 초기 회전값 저장
        lastRotation = target.rotation;
    }

    void Update()
    {
        // 현재 회전값 가져오기
        Quaternion currentRotation = target.rotation;

        // 이전 프레임과 현재 프레임의 회전값 차이를 계산
        Quaternion rotationDelta = currentRotation * Quaternion.Inverse(lastRotation);

        // 오브젝트를 회전
        objectToRotate.position = RotatePointAroundPivot(objectToRotate.position, target.position, rotationDelta);

        // 두 번째 오브젝트의 회전도 함께 적용
        objectToRotate.rotation = rotationDelta * objectToRotate.rotation;

        // 현재 회전값을 이전 회전값으로 업데이트
        lastRotation = currentRotation;
    }

    // 특정 점을 피벗을 중심으로 회전시키는 함수
    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
    {
        return rotation * (point - pivot) + pivot;
    }
}
