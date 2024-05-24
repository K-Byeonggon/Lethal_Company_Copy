using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SporeLizardDetect : MonoBehaviour
{
    SporeLizardAI lizard;

    private void Start()
    {
        lizard = transform.parent.GetComponent<SporeLizardAI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            lizard.bewareOf = other.transform;
            lizard.sawPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            lizard.sawPlayer = false;
        }
    }
}
