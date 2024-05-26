using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

public class PlayerHealth : LivingEntity
{
    [SerializeField] public GameObject DeadBody;
    [SerializeField] public PlayerController controller;
    /*private void OnEnable()
    {
        maxHealth = 100f;
        health = maxHealth;
        dead = false;
    }*/
    public override void OnStartServer()
    {
        maxHealth = 100f;
        health = maxHealth;
        dead = false;
        PlayerReference.Instance.AddPlayerToDic(this);
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
        CameraReference.Instance.SetActiveFirstOtherPlayerVirtualCamera();
        controller.CmdTeleport(new Vector3(0, 2000, 0));
        InstantiateDeadBody();
        //base.Die();
        Debug.Log("�÷��̾� ����");
    }
    [Command]
    public void InstantiateDeadBody()
    {
        GameObject deadBody = Instantiate(DeadBody);
        deadBody.transform.position = transform.position;
        MonsterReference.Instance.AddMonsterToList(deadBody);
        NetworkServer.Spawn(deadBody);
    }
    // �÷��̾ �׾����� ������ �˸�
    [Command]
    public void CmdPlayerDied()
    {
        //GameManager.Instance.
    }
}
