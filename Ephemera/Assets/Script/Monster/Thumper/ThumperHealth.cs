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
        //������ �ִ� ���� �ڱ� �ڽ��̰ų�, �ڽ��� �׾����� ����.
        if (!base.ApplyDamage(damageMessage)) return false;

        Debug.Log("����" + damageMessage.damage + " ��������");

        //���۴� ���ݹ����� ���ݹ��� ������ �������� �����Ѵ�.
        thumper.destination = damageMessage.damager.transform.position;
        thumper.setDesti = true;

        return true;
    }

    public override void Die()
    {
        base.Die();
        Debug.Log("���� ����");
    }

}
