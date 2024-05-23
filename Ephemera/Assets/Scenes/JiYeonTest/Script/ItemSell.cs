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

        //충돌한 오브젝트 정보 읽기

        //트리거 실행시 해당 오브젝트의 price를 플레이어의 지갑에 넣기

    }
}
