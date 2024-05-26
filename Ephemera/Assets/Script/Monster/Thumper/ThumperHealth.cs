using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumperHealth : LivingEntity
{
    [SerializeField]
    ThumperAI thumper;

    /*private void OnEnable()
    {
        maxHealth = 40f;
        health = maxHealth;
        dead = false;
    }*/

    public override void OnStartServer()
    {
        maxHealth = 40f;
        health = maxHealth;
        dead = false;
    }

    public override bool ApplyDamage(DamageMessage damageMessage)
    {
        //데미지 주는 것이 자기 자신이거나, 자신이 죽었으면 실패.
        if (!base.ApplyDamage(damageMessage)) return false;

        Debug.Log("썸퍼" + damageMessage.damage + " 피해입음");

        //썸퍼는 공격받으면 공격받은 방향을 목적지로 설정한다.
        thumper.destination = damageMessage.damager.transform.position;
        thumper.setDesti = true;

        return true;
    }

    public override void Die()
    {
        base.Die();
        Debug.Log("썸퍼 죽음");
    }

}
