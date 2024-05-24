using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSell : MonoBehaviour
{
    Rigidbody rigd;
    CapsuleCollider coll;

    Item currentItem;
    GameObject currentCollidedObject;
    public PlayerEx player;

    Item Item;

    void Awake()
    {
        rigd = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
        Item = GetComponent<Item>();

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("SellItem"))
        {
            return;
        }

        // 충돌한 오브젝트의 Item 컴포넌트 가져오기
        Item collidedItem = collision.gameObject.GetComponent<Item>();
        if (collidedItem != null)
        {
            // price 정보 읽기
            int itemPrice = collidedItem.itemPrice;

            // 트리거 실행시 해당 오브젝트의 price를 플레이어가 소지하고 있는 price에 더하기.
            // (여기서 Player 클래스나 Player의 price를 관리하는 변수가 필요함)
            // 예: player.price += itemPrice;

            Debug.Log("Item price: " + itemPrice);

            // 추가로 원하는 로직 실행


            //충돌한 오브젝트 데이터에 있는 price 정보 읽어내기

            //트리거 실행시 해당 오브젝트의 price를 플레이어가 소지하고있는 price에 더하기.

            if (!collision.gameObject.CompareTag("SellItem"))
            {
                return;
            }

            // 충돌한 오브젝트의 Item 컴포넌트 가져오기
            Item objcollidedItem = collision.gameObject.GetComponent<Item>();
            if (objcollidedItem != null)
            {
                // 현재 충돌한 아이템과 오브젝트를 저장
                currentItem = objcollidedItem;
                currentCollidedObject = collision.gameObject;

                Debug.Log("Collided with item: " + objcollidedItem.name + ", price: " + objcollidedItem.itemPrice);
            }
        }
    }

    public void OnSellButtonClicked()
    {
        if (currentItem != null && currentCollidedObject != null)
        {
            // Player의 coin 데이터에 아이템의 price를 더하기
            player.coin += currentItem.itemPrice;
            Debug.Log("Added " + currentItem.itemPrice + " coins to player. Total coins: " + player.coin);

            // 충돌한 오브젝트를 제거
            Destroy(currentCollidedObject);

            // 현재 아이템과 오브젝트 참조를 null로 설정
            currentItem = null;
            currentCollidedObject = null;
        }
    }
}
