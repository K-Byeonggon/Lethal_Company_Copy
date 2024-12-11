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
            if (!other.transform.TryGetComponent(out LivingEntity player) || player.IsDead)
                return;

            lizard.bewareOf = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (!other.transform.TryGetComponent(out LivingEntity player) || player.IsDead)
                return;

            lizard.bewareOf = null;
        }
    }
}
