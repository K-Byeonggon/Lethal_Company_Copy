using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDieCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<LivingEntity>(out LivingEntity entity))
        {
            entity.Die();
        }
    }
}
