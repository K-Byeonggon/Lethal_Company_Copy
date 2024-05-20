using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private PlayerEx player;
    public List<Slotdata> slots = new List<Slotdata>();
    public int maxSlot = 4;
    public int currentItemSlot = 0;

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

    public virtual void AddtoInventory(GameObject item)
    {
        if (slots[currentItemSlot].isEmpty)
        {
            slots[currentItemSlot].isEmpty = false;
            slots[currentItemSlot].slotObj = item;
            item.GetComponent<Item>().PickUp(player);
        }
    }

    public void RemovetoInventory()
    {
        if (!slots[currentItemSlot].isEmpty)
        {
            slots[currentItemSlot].slotObj.GetComponent<Item>().PickDown(player);
            slots[currentItemSlot].isEmpty = true;
            slots[currentItemSlot].slotObj = null;
        }
    }

    public virtual void ChangeItemSlot(int index)
    {
        if (index < 0 || index >= maxSlot)
            return;

        var currentItem = GetCurrentItem()?.GetComponent<Item>();
        if (currentItem != null && currentItem.isBothHandGrab)
        {
            Debug.Log("Cannot change item slot. Current item requires both hands.");
            return;
        }

        GetCurrentItem()?.SetActive(false);
        currentItemSlot = index;
        GetCurrentItem()?.SetActive(true);

        Debug.Log("Changed to item slot: " + currentItemSlot);
    }

    public bool IsUsable(GameObject usableItem)
    {
        foreach (var slot in slots)
        {
            if (slot.slotObj != null && slot.slotObj.CompareTag("UsableItem"))
            {
                Debug.Log("Usable item found: " + slot.slotObj.name);
                return true;
            }
        }
        Debug.Log("No usable item found.");
        return false;
    }

    public virtual GameObject GetCurrentItem()
    {
        return slots[currentItemSlot].slotObj;
    }
}
