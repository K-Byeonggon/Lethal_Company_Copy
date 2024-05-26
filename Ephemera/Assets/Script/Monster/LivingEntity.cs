using Mirror;
using System;
using UnityEngine;

//������ �ִ� ������Ʈ�� ��ũ��Ʈ���� LivingEntity ������Ʈ �����ͼ� ApplyDamage()����.
//������ DamageMessage ���� ���� .damage ����
public class LivingEntity : NetworkBehaviour, IDamageable
{
    public float maxHealth = 100f;
    [SyncVar]
    public float health;
    [SyncVar]
    public bool dead = false;

    public event Action OnDeath;

    public bool IsDead
    {
        get
        {
            if (health <= 0) return true;
            return false;
        }
    }

    /*protected virtual void OnEnable()
    {
        dead = false;
        health = maxHealth;
    }*/
    public override void OnStartServer()
    {
        dead = false;
        health = maxHealth;
    }

    public virtual bool ApplyDamage(DamageMessage damageMessage)
    {
        if (damageMessage.damager == gameObject || dead) return false;

        health -= damageMessage.damage;

        if (health <= 0) Die();

        return true;
    }

    public virtual void RestoreHealth(float newHealth)
    {
        if (dead) return;
        health += newHealth;
    }


    public virtual void Die()
    {
        if (OnDeath != null) OnDeath();
        dead = true;
        NetworkIdentity identity = GetComponent<NetworkIdentity>();
        GameManager.Instance.DestroyObject(identity);
    }
    
}