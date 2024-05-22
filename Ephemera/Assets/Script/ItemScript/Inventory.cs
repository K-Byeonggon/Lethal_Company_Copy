using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : NetworkBehaviour
{
    private int currentItemSlot = 0;
    private int maxSlot = 4;
    public List<Slotdata> slots = new List<Slotdata>();

    [SerializeField] Transform pickTransform;

    public GameObject GetCurrentItem => slots[currentItemSlot].slotObjComponent.gameObject;
    public Item GetCurrentItemComponent => slots[currentItemSlot].slotObjComponent;
    public bool IsOutRange(int index) => (index < 0 || index >= maxSlot) ? true : false;

    private void Awake()
    {
        for (int i = 0; i < maxSlot; i++)
        {
            slots.Add(new Slotdata());
        }
    }
    public void AddItem(GameObject item)
    {
        if (slots[currentItemSlot].isEmpty == true)
        {
            slots[currentItemSlot].isEmpty = false;
            slots[currentItemSlot].slotObjComponent = item.GetComponent<Item>();
            item.GetComponent<Item>().PickUp(pickTransform);
        }
    }
    public void RemoveItem()
    {
        if (slots[currentItemSlot].isEmpty == false)
        {
            slots[currentItemSlot].isEmpty = true;
            slots[currentItemSlot].slotObjComponent.PickDown(pickTransform);
            slots[currentItemSlot].slotObjComponent = null;
        }
    }
    public void ChangeItemSlot(int index)
    {
        if (IsOutRange(index)) return;

        var currentItem = GetCurrentItemComponent;
        if (currentItem != null && currentItem.IsBothHandGrab) return;

        CmdSetCurrentItemActive(false);
        currentItemSlot = index;
        CmdSetCurrentItemActive(true);
    }
    public void UseItem()
    {
        GetCurrentItemComponent?.UseItem();
    }

    #region Command Function
    [Command]
    public void CmdSetCurrentItemActive(bool isActive)
    {
        OnClientSetCurrentItemActive(isActive);
    }
    #endregion
    #region ClientRpc Function
    [ClientRpc]
    public void OnClientSetCurrentItemActive(bool isActive)
    {
        GetCurrentItem?.SetActive(isActive);
    }
    #endregion
}
