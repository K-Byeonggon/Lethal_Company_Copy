using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSell : MonoBehaviour
{
    Rigidbody rigd;
    CapsuleCollider coll;

    Item Item;

    void Awake()
    {
        rigd = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
        Item = coll.GetComponent<Item>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("SellItem"))
        {
            return;
        }

        //�浹�� ������Ʈ ���� �б�

        //Ʈ���� ����� �ش� ������Ʈ�� price�� �÷��̾��� ������ �ֱ�

    }
}
