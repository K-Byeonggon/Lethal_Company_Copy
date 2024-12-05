using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSell : MonoBehaviour
{
    Rigidbody rigd;
    CapsuleCollider coll;

    List<Item> collidedItems = new List<Item>();
    List<GameObject> collidedObjects = new List<GameObject>();

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

        // �浹�� ������Ʈ�� Item ������Ʈ ��������
        Item collidedItem = collision.gameObject.GetComponent<Item>();
        if (collidedItem != null && !collidedObjects.Contains(collision.gameObject))
        {
            // �浹�� �����۰� ������Ʈ�� ����Ʈ�� �߰�
            collidedItems.Add(collidedItem);
            collidedObjects.Add(collision.gameObject);

            Debug.Log("Collided with item: " + collidedItem.name + ", price: " + collidedItem.ItemPrice);
        }
    }

    public void OnSellButtonClicked()
    {
        if (collidedItems.Count > 0)
        {
            int totalPrice = 0;

            // ��� �浹�� �������� price �ջ�
            foreach (var item in collidedItems)
            {
                totalPrice += item.ItemPrice;
            }

            // Player�� coin �����Ϳ� �� price�� ���ϱ�
            GameManager.Instance.CurrentMoney += totalPrice;

            Debug.Log("Added " + totalPrice + " coins to player. Total coins: " + GameManager.Instance.CurrentMoney);

            // ��� �浹�� ������Ʈ ����
            foreach (var obj in collidedObjects)
            {
                Destroy(obj);
            }

            // ����Ʈ �ʱ�ȭ
            collidedItems.Clear();
            collidedObjects.Clear();
        }
    }
}
