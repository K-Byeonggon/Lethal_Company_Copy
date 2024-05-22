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

            // �� ���ʹϾ� ������ ������� ȸ���� ��Ÿ���� ���ʹϾ� ���
            Quaternion relativeRotationQuaternion = lookAt * Quaternion.Inverse(transform.rotation);

            // ������� ȸ�� ���ʹϾ��� ����� ������ ��ȯ
            float angle;
            Vector3 axis;
            relativeRotationQuaternion.ToAngleAxis(out angle, out axis);

            // ���ο� ���ʹϾ� ����
            Quaternion newQuaternion = Quaternion.AngleAxis(angle, axis);

            // ��� ���
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
