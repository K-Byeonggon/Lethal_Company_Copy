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
    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log(collision.gameObject.name);
    //    if (collision.gameObject.CompareTag("Mine"))
    //    {
    //        Debug.Log("isDie");
    //        health = 0;
    //        Vector3 playerPosition = this.transform.position;  // ���� �÷��̾��� ��ġ�� ����
    //        Quaternion playerRotation = this.transform.rotation;  // ���� �÷��̾��� ȸ���� ����

    //        Instantiate(ragdoll, playerPosition, playerRotation);  // �÷��̾� ��ġ�� ragdoll ������ ����
    //        //ragdoll.SetActive(true);
    //        Destroy(this.gameObject);  // ���� �÷��̾� ������Ʈ ����
    //    }
    //}
}
