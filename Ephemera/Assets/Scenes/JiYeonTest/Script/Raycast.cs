using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    [SerializeField] public const float length = 10.0f;
    [SerializeField] LayerMask layerRed;

    void Start()
    {
            
    }

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * length, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, length, layerRed))
            Debug.Log(gameObject.name);
    }


}
