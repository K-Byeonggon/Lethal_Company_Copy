using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JetPack_Item : Item, IItemUsable
{
    [SerializeField]
    public Rigidbody rb;
    private float jumpForce = 10f;
    private bool isJetPackEquipped = false;
    int jumpcount;
    void Start()
    {
        isJetPackEquipped = true;
    }

    void Update()
    {
        UseItem();
    }

    public override void UseItem()
    {
        // 제트팩이 장착되어 있고, 스페이스 바를 누르고, 두 번째 점프를 하지 않은 경우
        if (isJetPackEquipped && Input.GetKeyDown(KeyCode.Space) && jumpcount<2)
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.velocity= Vector3.zero;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        jumpcount++;
    }

    private void OnTriggerEnter(Collider other)
    {
        jumpcount = 0;
    }

    public void EquipJetPack()
    {
        isJetPackEquipped = true;
    }

    public void UnequipJetPack()
    {
        isJetPackEquipped = false;
    }
}
