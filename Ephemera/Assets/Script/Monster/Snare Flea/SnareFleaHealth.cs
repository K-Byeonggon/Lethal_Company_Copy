using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnareFleaHealth : LivingEntity
{
    [SerializeField]
    SnareFleaAI snare;

    /*private void OnEnable()
    {
        maxHealth = 20f;
        health = maxHealth;
        dead = false;
    }*/
    public override void OnStartServer()
    {
        maxHealth = 20f;
        health = maxHealth;
        dead = false;
    }

    public override bool ApplyDamage(DamageMessage damageMessage)
    {
        //데미지 주는 것이 자기 자신이거나, 자신이 죽었으면 실패.
        if (!base.ApplyDamage(damageMessage)) return false;

        Debug.Log("올가미벌레" + damageMessage.damage + " 피해입음");

        //올가미벌레는 공격받으면 도망치다가 천장에 붙는다.
        snare.isAttacked = true;

        return true;
    }

    public override void Die()
    {
        base.Die();
        Debug.Log("올가미벌레 죽음");
    }
}
