using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    float currentForce = 0f;

    [SerializeField]
    float targetForce = 0f;

    [SerializeField]
    Vector3 m_offset = Vector3.zero;

    Quaternion m_originRot;
    Vector3 t_originEuler;

    // Start is called before the first frame update
    void Start()
    {
        m_originRot = transform.rotation;
        t_originEuler = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentForce > 0)
        {
            Quaternion rotation = GetRandomQuaternion();
            transform.rotation = Quaternion.RotateTowards(m_originRot, rotation, currentForce * Time.deltaTime);
            currentForce = Mathf.Lerp(currentForce, 0f, 0.1f);
        }
        else if(transform.rotation == m_originRot)
        {
            return;
        }
        else if(currentForce <= 0)
        {
            transform.rotation = m_originRot;
        }
    }
    private Quaternion GetRandomQuaternion()
    {
        float rotX = Random.Range(-m_offset.x, m_offset.x);
        float rotY = Random.Range(-m_offset.y, m_offset.y);
        float rotZ = Random.Range(-m_offset.z, m_offset.z);
        Vector3 randomRot = t_originEuler + new Vector3(rotX, rotY, rotZ);
        return Quaternion.Euler(randomRot);
    }
    public void StartWarp()
    {
        currentForce = targetForce;
    }
}
