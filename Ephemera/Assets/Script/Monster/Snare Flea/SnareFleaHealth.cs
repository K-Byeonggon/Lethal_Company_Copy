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
        //������ �ִ� ���� �ڱ� �ڽ��̰ų�, �ڽ��� �׾����� ����.
        if (!base.ApplyDamage(damageMessage)) return false;

        Debug.Log("�ð��̹���" + damageMessage.damage + " ��������");

        //�ð��̹����� ���ݹ����� ����ġ�ٰ� õ�忡 �ٴ´�.
        snare.isAttacked = true;

        return true;
    }

    public override void Die()
    {
        base.Die();
        Debug.Log("�ð��̹��� ����");
    }
}
