using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DamageMessage
{
    public GameObject damager;
    public float damage;

    public Vector3 hitPoint;
    public Vector3 hitNormal;
}


public interface IDamageable
{
    public bool ApplyDamage(DamageMessage damageMessage);
}
