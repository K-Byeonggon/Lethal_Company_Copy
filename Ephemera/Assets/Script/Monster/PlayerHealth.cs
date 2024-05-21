using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : LivingEntity
{
    private void OnEnable()
    {
        maxHealth = 100f;
        health = maxHealth;
        dead = false;
    }

    public override bool ApplyDamage(DamageMessage damageMessage)
    {
        //데미지 주는 것이 자기 자신이거나, 자신이 죽었으면 실패.
        if (!base.ApplyDamage(damageMessage)) return false;

        Debug.Log("플레이어" + damageMessage.damage + " 피해입음");
        return true;
    }

    public override void Die()
    {
        base.Die();
        Debug.Log("플레이어 죽음");
    }
}
