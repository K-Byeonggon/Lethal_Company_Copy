using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCeiling : MonoBehaviour
{
    SnareFleaAI snare;
    private void Start()
    {
        snare = transform.parent.GetComponent<SnareFleaAI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            Debug.Log("천장임");
            snare.atCeiling = true;

            snare.SetDefault();
            //Rigidbody rigidbody = transform.parent.GetComponent<Rigidbody>();
            //rigidbody.velocity = Vector3.zero;
            //rigidbody.useGravity = false;
            //snare.sawPlayer = false;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            Debug.Log("천장아님");
            snare.atCeiling = false;
        }
    }
}
