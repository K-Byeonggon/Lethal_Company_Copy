using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private PlayerEx player;
    public List<Slotdata> slots = new List<Slotdata>();
    public int maxSlot = 4;
    public int currentItemSlot = 0;
    [SerializeField] public Transform pickedItem;

    private void Awake()
    {
        player = GetComponent<PlayerEx>();
        if (player == null)
        {
            Debug.LogError("PlayerEx component not found on this GameObject.");
        }

        for (int i = 0; i < maxSlot; i++)
        {
            slots.Add(new Slotdata());
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("GetMouseButtonDown");
            UseItem();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Detach item");
            RemovetoInventory(this.gameObject);
        }
    }
    public virtual void AddtoInventory(GameObject item)
    {
        if (slots[currentItemSlot].isEmpty)
        {
            slots[currentItemSlot].isEmpty = false;
            slots[currentItemSlot].slotObj = item;
            slots[currentItemSlot].slotObjComponent = item.GetComponent<Item>();
            item.GetComponent<Item>().PickUp(this);
        }
    }

    public void RemovetoInventory(GameObject item)
    {
        if (!slots[currentItemSlot].isEmpty)
        {
            slots[currentItemSlot].slotObj.GetComponent<Item>().PickDown(this);
            slots[currentItemSlot].isEmpty = true;
            slots[currentItemSlot].slotObj = null;
            slots[currentItemSlot].slotObjComponent = null;
        }
    }

    public virtual void ChangeItemSlot(int index)
    {
        if (index < 0 || index >= maxSlot)
            return;

        var currentItem = GetCurrentItem()?.GetComponent<Item>();
        if (currentItem != null && currentItem.IsBothHandGrab)
        {
            return;
        }

        GetCurrentItem()?.SetActive(false);
        currentItemSlot = index;
        GetCurrentItem()?.SetActive(true);

        Debug.Log("슬롯 :  " + currentItemSlot);
    }

    //아이템 사용
    public void UseItem()
    {
        GetCurrentItemComponent()?.UseItem();
    }


    public virtual GameObject GetCurrentItem()
    {
        return slots[currentItemSlot].slotObj;
    }
    public virtual Item GetCurrentItemComponent()
    {
        return slots[currentItemSlot].slotObjComponent;
    }
}
