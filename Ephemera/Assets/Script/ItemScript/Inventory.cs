using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Inventory : NetworkBehaviour
{
    private int currentItemSlot = 0;
    private int maxSlot = 4;
    public List<Slotdata> slots = new List<Slotdata>();

    [SerializeField] Transform pickTransform;

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
        {
            Debug.Log("Item is Null");
            return;
        }
        if (slots[currentItemSlot].isEmpty == true)
        {
            Debug.Log($"pickTransform : {pickTransform}");
            slots[currentItemSlot].isEmpty = false;
            slots[currentItemSlot].slotObjComponent = item.GetComponent<Item>();
            //PickUp(item.transform);
            PickUp(item.GetComponent<NetworkIdentity>());
        }
    }
    public void ThrowItem()
    {
        if (slots[currentItemSlot].isEmpty == false)
        {
            slots[currentItemSlot].isEmpty = true;
            //PickDown(slots[currentItemSlot].slotObjComponent.transform);
            PickDown(slots[currentItemSlot].slotObjComponent.GetComponent<NetworkIdentity>());
            slots[currentItemSlot].slotObjComponent = null;
        }
    }
    public void ThrowAllItem()
    {
        foreach (var slot in slots)
        {
            if (slot.isEmpty == false)
            {
                slot.isEmpty = true;
                //PickDown(slots[currentItemSlot].slotObjComponent.transform);
                PickDown(slot.slotObjComponent.GetComponent<NetworkIdentity>());
                slot.slotObjComponent = null;
            }
        }
        
    }
    [Command]
    public void RemoveItem()
    {
        if (slots[currentItemSlot].isEmpty == false)
        {
            slots[currentItemSlot].isEmpty = true;
            NetworkServer.Destroy(slots[currentItemSlot].slotObjComponent.gameObject);
            slots[currentItemSlot].slotObjComponent = null;
        }
        
    }

    public void ChangeItemSlot(int index)
    {
        if (IsOutRange(index)) return;

        var currentItem = GetCurrentItemComponent;
        if (currentItem != null && currentItem.IsBothHandGrab) return;

        CmdSetCurrentItemActive(currentItemSlot, false);
        currentItemSlot = index;
        CmdSetCurrentItemActive(currentItemSlot, true);
        UIController.Instance.ui_Game.ItemSelection(index);
    }
    [Command] public void UseItem()
    {
        GetCurrentItemComponent?.UseItem();
    }

    #region Command Function
    [Command(requiresAuthority = false)] public void CmdSetCurrentItemActive(int itemSlotIndex, bool isActive)
    {
        Item item = slots[itemSlotIndex].slotObjComponent;
        if (item != null)
            OnClientSetCurrentItemActive(itemSlotIndex, isActive);
    }
    #endregion

    #region ClientRpc Function
    [Server] public void OnClientSetCurrentItemActive(int itemSlotIndex, bool isActive)
    {
        if(slots[itemSlotIndex].slotObjComponent != null)
        {
            slots[itemSlotIndex].slotObjComponent.SetRendererActive(isActive);
        }
        
    }
    #endregion


    [Command]
    public void PickUp(NetworkIdentity itemIdentity)
    {
        /*var itemComponent = item.GetComponent<Item>();

        itemComponent.itemCollider.enabled = false;
        itemComponent.rigid.isKinematic = true;
        itemComponent.rigid.useGravity = false;
        itemComponent.rigid.velocity = Vector3.zero;
        itemComponent.rigid.angularVelocity = Vector3.zero;*/

        OnClientSetParent(itemIdentity);
    }
    [Command]
    public void PickDown(NetworkIdentity itemIdentity)
    {
        OnClientUnsetParent(itemIdentity);
        /*var itemComponent = item.GetComponent<Item>();

        itemComponent.itemCollider.enabled = true;
        itemComponent.rigid.isKinematic = false;
        itemComponent.rigid.useGravity = true;
        item.position = transform.position + transform.forward;

        OnClientUnsetParent(item);
        itemComponent.rigid.AddForce(transform.forward * 5.0f, ForceMode.Impulse);*/
    }
    [ClientRpc]
    public void OnClientSetParent(NetworkIdentity itemIdentity)
    {
        var itemComponent = itemIdentity.GetComponent<Item>();

        itemComponent.itemCollider.enabled = false;
        itemComponent.rigid.isKinematic = true;
        itemComponent.rigid.useGravity = false;
        itemComponent.rigid.velocity = Vector3.zero;
        itemComponent.rigid.angularVelocity = Vector3.zero;


        itemComponent.transform.parent = pickTransform;
        itemComponent.transform.position = pickTransform.position;
        itemComponent.transform.rotation = pickTransform.rotation;
    }
    [ClientRpc]
    public void OnClientUnsetParent(NetworkIdentity itemIdentity)
    {
        var itemComponent = itemIdentity.GetComponent<Item>();

        itemComponent.itemCollider.enabled = true;
        itemComponent.rigid.isKinematic = false;
        itemComponent.rigid.useGravity = true;
        itemIdentity.transform.position = transform.position + transform.forward;

        itemIdentity.transform.parent = null;
        itemComponent.rigid.AddForce(transform.forward * 5.0f, ForceMode.Impulse);

        //item.parent = null;
    }
}
