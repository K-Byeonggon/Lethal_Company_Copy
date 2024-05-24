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
        //데미지 주는 것이 자기 자신이거나, 자신이 죽었으면 실패.
        if (!base.ApplyDamage(damageMessage)) return false;

        Debug.Log("플레이어" + damageMessage.damage + " 피해입음");
        return true;
    }

    public override void Die()
    {
        base.Die();
        Debug.Log("플레이어 죽음");
    }
    public void UpdateStamina()
    {
        if (this != null)
        {
            stamina -= Runtime*Time.deltaTime;
            Debug.Log("스태미나가 줄어드는가? " + stamina);
        }
    }
    public void RefillStamina()
    {
        if (this != null)
        {
            stamina += Runtime*Time.deltaTime;
            if (stamina >= maxstamina)
            {
                Debug.Log("스태미나가 돌아오는가?" + stamina);
                stamina = maxstamina;
                
            }
        }
    }

}
