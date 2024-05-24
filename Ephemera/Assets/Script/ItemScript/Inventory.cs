using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : NetworkBehaviour
{
    private int currentItemSlot = 0;
    private int maxSlot = 4;
    public List<Slotdata> slots = new List<Slotdata>();

    [SerializeField] Transform pickTransform;



    private void Update()
    {
        if (!isLocalPlayer)
            return;
        Debug.Log($"conn : {netId}");
        Debug.Log($"currentItemSlot : {currentItemSlot}");
        if (slots[currentItemSlot].isEmpty != true)
        {
            GetCurrentItemComponent.CmdChangePosRot(pickTransform);
        }
    }

    public Item GetCurrentItemComponent
    {
        get
        {
            if (slots[currentItemSlot].isEmpty != true)
            {
                return slots[currentItemSlot].slotObjComponent;
            }
            return null;
        }
    }
    public bool IsOutRange(int index) => (index < 0 || index >= maxSlot) ? true : false;


    public override void OnStartClient()
    {
        for (int i = 0; i < maxSlot; i++)
        {
            slots.Add(new Slotdata());
        }
        ChangeItemSlot(0);
    }
    public void AddItem(GameObject item)
    {
        if (item == null)
            return;
        if (slots[currentItemSlot].isEmpty == true)
        {
            slots[currentItemSlot].isEmpty = false;
            slots[currentItemSlot].slotObjComponent = item.GetComponent<Item>();
            slots[currentItemSlot].slotObjComponent.PickUp(pickTransform);
            slots[currentItemSlot].slotObjComponent.CmdChangePosRot(pickTransform);
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
        UIController.Instance.ui_Game.ItemSelection(index);
    }
    public void UseItem()
    {
        GetCurrentItemComponent?.UseItem();
    }

    #region Command Function
    [Command] public void CmdSetCurrentItemActive(bool isActive)
    {
        Item item = GetCurrentItemComponent;
        if (item != null)
            OnClientSetCurrentItemActive(item.gameObject, isActive);
    }
    #endregion
    
    #region ClientRpc Function
    [ClientRpc] public void OnClientSetCurrentItemActive(GameObject go, bool isActive)
    {
        go?.SetActive(isActive);
    }
    #endregion
}
