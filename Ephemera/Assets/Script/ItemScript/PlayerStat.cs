using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : LivingEntity
{
    [SerializeField]
    public GameObject ragdoll;
    private void OnEnable()
    {
        maxHealth = 100f;
        health = maxHealth;
        dead = false;
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
    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log(collision.gameObject.name);
    //    if (collision.gameObject.CompareTag("Mine"))
    //    {
    //        Debug.Log("isDie");
    //        health = 0;
    //        Vector3 playerPosition = this.transform.position;  // 현재 플레이어의 위치를 저장
    //        Quaternion playerRotation = this.transform.rotation;  // 현재 플레이어의 회전을 저장

    //        Instantiate(ragdoll, playerPosition, playerRotation);  // 플레이어 위치에 ragdoll 프리팹 생성
    //        //ragdoll.SetActive(true);
    //        Destroy(this.gameObject);  // 현재 플레이어 오브젝트 제거
    //    }
    //}
}
