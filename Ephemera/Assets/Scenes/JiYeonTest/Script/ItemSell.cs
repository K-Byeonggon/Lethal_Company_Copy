using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSell : MonoBehaviour
{
    Rigidbody rigd;
    CapsuleCollider coll;

    List<Item> collidedItems = new List<Item>();
    List<GameObject> collidedObjects = new List<GameObject>();
    public PlayerEx player;


    void Awake()
    {
        rigd = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("SellItem"))
        {
            return;
        }

        // 충돌한 오브젝트의 Item 컴포넌트 가져오기
        Item collidedItem = collision.gameObject.GetComponent<Item>();
        if (collidedItem != null && !collidedObjects.Contains(collision.gameObject))
        {
            // 충돌한 아이템과 오브젝트를 리스트에 추가
            collidedItems.Add(collidedItem);
            collidedObjects.Add(collision.gameObject);

            Debug.Log("Collided with item: " + collidedItem.name + ", price: " + collidedItem.itemPrice);
        }
    }

    public void OnSellButtonClicked()
    {
        if (collidedItems.Count > 0)
        {
            int totalPrice = 0;

            // 모든 충돌한 아이템의 price 합산
            foreach (var item in collidedItems)
            {
                totalPrice += item.itemPrice;
            }

            // Player의 coin 데이터에 총 price를 더하기
            player.coin += totalPrice;
            Debug.Log("Added " + totalPrice + " coins to player. Total coins: " + player.coin);

            // 모든 충돌한 오브젝트 제거
            foreach (var obj in collidedObjects)
            {
                Destroy(obj);
            }

            // 리스트 초기화
            collidedItems.Clear();
            collidedObjects.Clear();
        }
    }
}
