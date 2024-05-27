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
    [SerializeField] public CharacterController characterController;
    [SerializeField] public Inventory inventory;
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
    }
    public override void OnStartClient()
    {
        PlayerReference.Instance.AddPlayerToDic(this);
    }
    public override void OnStartLocalPlayer()
    {
        PlayerReference.Instance.AddLocalPlayer(this);
    }

    public override bool ApplyDamage(DamageMessage damageMessage)
    {
        //데미지 주는 것이 자기 자신이거나, 자신이 죽었으면 실패.
        if (!base.ApplyDamage(damageMessage)) return false;

        Debug.Log("플레이어" + damageMessage.damage + " 피해입음");
        return true;
    }
    public void Revive()
    {
        maxHealth = 100f;
        health = maxHealth;
        dead = false;
    }

    public override void Die()
    {
        inventory.ThrowAllItem();

        dead = true;
        controller.PlayerDie();
        //CameraReference.Instance.SetActiveFirstOtherPlayerVirtualCamera();
        controller.CmdTeleport(new Vector3(0, 2000, 0));
        InstantiateDeadBody();
        //base.Die();
        CmdPlayerDied();
        Debug.Log("플레이어 죽음");
    }

    public void SetActiveCharacterController(bool isActive)
    {
        if (isLocalPlayer)
        {
            characterController.enabled = isActive;
        }
    }

    [Command(requiresAuthority = false)]
    public void InstantiateDeadBody()
    {
        GameObject deadBody = Instantiate(DeadBody);
        deadBody.transform.position = transform.position;
        MonsterReference.Instance.AddMonsterToList(deadBody);
        NetworkServer.Spawn(deadBody);
    }
    // 플레이어가 죽었음을 서버에 알림
    [Command(requiresAuthority = false)]
    public void CmdPlayerDied()
    {
        GameManager.Instance.PlayerDieEvent();
    }
}
