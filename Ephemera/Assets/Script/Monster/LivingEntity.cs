using Mirror;
using System;
using UnityEngine;

//데미지 주는 오브젝트의 스크립트에서 LivingEntity 컴포넌트 가져와서 ApplyDamage()실행.
//실행전 DamageMessage 새로 만들어서 .damage 편집
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