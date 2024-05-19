using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    bool hasDestination = false;
    Vector3 Destination = Vector3.zero;
    Quaternion lookAt;

    bool landingPlanet = false;
    private void FixedUpdate()
    {
        if (hasDestination == true)
        {
            Debug.Log("isLerp");
            transform.rotation = Quaternion.Slerp(transform.rotation, lookAt, 0.01f);

            // 두 쿼터니언 사이의 상대적인 회전을 나타내는 쿼터니언 계산
            Quaternion relativeRotationQuaternion = lookAt * Quaternion.Inverse(transform.rotation);

            // 상대적인 회전 쿼터니언을 각축과 각도로 변환
            float angle;
            Vector3 axis;
            relativeRotationQuaternion.ToAngleAxis(out angle, out axis);

            // 새로운 쿼터니언 생성
            Quaternion newQuaternion = Quaternion.AngleAxis(angle, axis);

            // 결과 출력
            Debug.Log("New Quaternion: " + newQuaternion);
            if (newQuaternion.x < 0.1f)
            {
                hasDestination = false;
                landingPlanet = true;
            }
        }
        if(landingPlanet == true)
        {
            Debug.Log("isLanding");
            transform.position = Vector3.Slerp(transform.position, Destination, 0.01f);
        }
    }

    public void SetDestination()
    {
        Destination = Vector3.zero;
        Vector3 targetDirection = Destination - transform.position;
        lookAt = Quaternion.FromToRotation(transform.forward, targetDirection);
        hasDestination = true;

        Debug.Log(Destination);
        Debug.Log(lookAt);
    }
}
