using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YipeeHealth : LivingEntity
{
    YipeeAI yipee;

    private void OnEnable()
    {
        //비축벌레는 삽 2대에 죽는다.
        maxHealth = 20f;
        health = maxHealth;
        dead = false;
    }


    private void Start()
    {
        yipee = GetComponent<YipeeAI>();
    }

    public override bool ApplyDamage(DamageMessage damageMessage)
    {
        //데미지 주는 것이 자기 자신이거나, 자신이 죽었으면 실패.
        if(!base.ApplyDamage(damageMessage)) return false;
        
        Debug.Log("비축벌레" + damageMessage.damage + " 피해입음");

        //비축벌레는 공격받으면 공격한자를 공격하는 공격시퀀스가 실행됨.
        yipee.isAttacked = true;
        if(yipee.player == null) { yipee.player = damageMessage.damager.transform; }

        return true;
    }

    public override void Die()
    {
        base.Die();
        Debug.Log("비축벌레 죽음");
    }

}
