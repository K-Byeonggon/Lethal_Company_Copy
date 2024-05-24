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

        // �浹�� ������Ʈ�� Item ������Ʈ ��������
        Item collidedItem = collision.gameObject.GetComponent<Item>();
        if (collidedItem != null)
        {
            // price ���� �б�
            int itemPrice = collidedItem.itemPrice;

            // Ʈ���� ����� �ش� ������Ʈ�� price�� �÷��̾ �����ϰ� �ִ� price�� ���ϱ�.
            // (���⼭ Player Ŭ������ Player�� price�� �����ϴ� ������ �ʿ���)
            // ��: player.price += itemPrice;

            Debug.Log("Item price: " + itemPrice);

            // �߰��� ���ϴ� ���� ����


            //�浹�� ������Ʈ �����Ϳ� �ִ� price ���� �о��

            //Ʈ���� ����� �ش� ������Ʈ�� price�� �÷��̾ �����ϰ��ִ� price�� ���ϱ�.

            if (!collision.gameObject.CompareTag("SellItem"))
            {
                return;
            }

            // �浹�� ������Ʈ�� Item ������Ʈ ��������
            Item objcollidedItem = collision.gameObject.GetComponent<Item>();
            if (objcollidedItem != null)
            {
                // ���� �浹�� �����۰� ������Ʈ�� ����
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
            // Player�� coin �����Ϳ� �������� price�� ���ϱ�
            player.coin += currentItem.itemPrice;
            Debug.Log("Added " + currentItem.itemPrice + " coins to player. Total coins: " + player.coin);

            // �浹�� ������Ʈ�� ����
            Destroy(currentCollidedObject);

            // ���� �����۰� ������Ʈ ������ null�� ����
            currentItem = null;
            currentCollidedObject = null;
        }
    }
}
