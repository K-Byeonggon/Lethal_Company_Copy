using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumperHealth : LivingEntity
{
    ThumperAI thumper;

    private void OnEnable()
    {
        maxHealth = 40f;
        health = maxHealth;
        dead = false;
    }


    private void Start()
    {
        thumper = GetComponent<ThumperAI>();
    }

    public override bool ApplyDamage(DamageMessage damageMessage)
    {
        //������ �ִ� ���� �ڱ� �ڽ��̰ų�, �ڽ��� �׾����� ����.
        if (!base.ApplyDamage(damageMessage)) return false;

        Debug.Log("����" + damageMessage.damage + " ��������");

        //���۴� ���ݹ����� ���ݹ��� �������� ���ƺ���.


        return true;
    }

    public override void Die()
    {
        base.Die();
        Debug.Log("���� ����");
    }

}
