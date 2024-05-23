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

    //이전 위치
    private Vector3 previousPosition;
    private Quaternion previousRotation;

    //현재 소지 아이템
    public Transform loadedObject;

    private void Update()
    {
        
    }

    public GameObject GetCurrentItem
    {
        get
        {
            if (GetCurrentItemComponent != null)
            {
                return GetCurrentItemComponent.gameObject;
            }
            return null;
        }
    }
    public Item GetCurrentItemComponent
    {
        get
        {
            if (slots[currentItemSlot] != null)
            {
                if (slots[currentItemSlot].slotObjComponent != null)
                {
                    return slots[currentItemSlot].slotObjComponent;
                }
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
        if (slots[currentItemSlot].isEmpty == true)
        {
            slots[currentItemSlot].isEmpty = false;
            slots[currentItemSlot].slotObjComponent = item.GetComponent<Item>();
            item.GetComponent<Item>().PickUp(pickTransform);

            loadedObject = item.transform;
            SetItemPosRot(item);
        }
    }
    public void RemoveItem()
    {
        if (slots[currentItemSlot].isEmpty == false)
        {
            loadedObject = null;
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

        loadedObject = null;
        CmdSetCurrentItemActive(false);
        currentItemSlot = index;
        CmdSetCurrentItemActive(true);
        loadedObject = GetCurrentItem.transform;
        SetItemPosRot(GetCurrentItem);
        UIController.Instance.ui_Game.ItemSelection(index);
    }
    public void UseItem()
    {
        GetCurrentItemComponent?.UseItem();
    }

    #region Server Function
    [Server] public void OnServerChangePosition(Vector3 vec)
    {
        OnClientChangePosition(vec);
    }
    [Server] public void OnServerChangeRotation(Quaternion quaternion)
    {
        OnClientChangeRotation(quaternion);
    }
    #endregion
    #region Command Function
    [Command] public void CmdSetCurrentItemActive(bool isActive)
    {
        OnClientSetCurrentItemActive(GetCurrentItem, isActive);
    }
    #endregion
    #region ClientRpc Function
    [ClientRpc] public void OnClientSetCurrentItemActive(GameObject go, bool isActive)
    {
        go?.SetActive(isActive);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="vec">현재 pickTransform의 위치</param>
    [ClientRpc] public void OnClientChangePosition(Vector3 vec)
    {
        Vector3 moveVector = vec - previousPosition;
        transform.position = vec;
        loadedObject.position += moveVector;
        previousPosition = vec;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="quaternion">현재 pickTransform의 회전값</param>
    [ClientRpc] public void OnClientChangeRotation(Quaternion quaternion)
    {
        Quaternion currentRotation = quaternion;
        Quaternion rotationDelta = currentRotation * Quaternion.Inverse(previousRotation);

        loadedObject.position = RotatePointAroundPivot(loadedObject.position, transform.position, rotationDelta);
        loadedObject.rotation = rotationDelta * loadedObject.rotation;

        previousRotation = currentRotation;
    }
    #endregion

    // 특정 점을 피벗을 중심으로 회전시키는 함수
    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
    {
        return rotation * (point - pivot) + pivot;
    }
    private void SetItemPosRot(GameObject item)
    {
        previousPosition = pickTransform.position;
        previousRotation = pickTransform.rotation;
        item.transform.position = pickTransform.position;
        item.transform.rotation = pickTransform.rotation;
    }
}
