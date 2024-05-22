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
        //������ �ִ� ���� �ڱ� �ڽ��̰ų�, �ڽ��� �׾����� ����.
        if (!base.ApplyDamage(damageMessage)) return false;

        Debug.Log("�÷��̾�" + damageMessage.damage + " ��������");
        return true;
    }

    public override void Die()
    {
        base.Die();
        Debug.Log("�÷��̾� ����");
    }
}
