using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YipeeHealth : LivingEntity
{
    [SerializeField]
    YipeeAI yipee;

    /*private void OnEnable()
    {
        //��������� �� 2�뿡 �״´�.
        maxHealth = 20f;
        health = maxHealth;
        dead = false;
    }*/
    public override void OnStartServer()
    {
        //��������� �� 2�뿡 �״´�.
        maxHealth = 20f;
        health = maxHealth;
        dead = false;
    }

    public override bool ApplyDamage(DamageMessage damageMessage)
    {
        //������ �ִ� ���� �ڱ� �ڽ��̰ų�, �ڽ��� �׾����� ����.
        if(!base.ApplyDamage(damageMessage)) return false;
        
        Debug.Log("�������" + damageMessage.damage + " ��������");

        //��������� ���ݹ����� �������ڸ� �����ϴ� ���ݽ������� �����.
        yipee.isAttacked = true;
        if(yipee.player == null) { yipee.player = damageMessage.damager.transform; }

        return true;
    }

    public override void Die()
    {
        base.Die();
        Debug.Log("������� ����");
    }
}
