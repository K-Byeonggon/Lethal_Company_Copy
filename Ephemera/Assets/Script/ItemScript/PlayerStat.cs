using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStat : LivingEntity
{
    [SerializeField]
    public GameObject ragdoll;
    public float stamina;
    public float maxstamina;
    private float Runtime;
    private void OnEnable()
    {
        maxHealth = 100f;
        health = maxHealth;
        dead = false;
        maxstamina = 5f;
        stamina = 5f;
        Runtime = 1f;
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
    public void UpdateStamina()
    {
        if (this != null)
        {
            stamina -= Runtime*Time.deltaTime;
            Debug.Log("���¹̳��� �پ��°�? " + stamina);
        }
    }
    public void RefillStamina()
    {
        if (this != null)
        {
            stamina += Runtime*Time.deltaTime;
            if (stamina >= maxstamina)
            {
                Debug.Log("���¹̳��� ���ƿ��°�?" + stamina);
                stamina = maxstamina;
                
            }
        }
    }

}
