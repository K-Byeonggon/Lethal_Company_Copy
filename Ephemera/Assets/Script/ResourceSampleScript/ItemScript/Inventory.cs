using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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
        for (int i = 0; i < maxSlot; i++)
        {
            slots.Add(new Slotdata());
        }
    }
    public virtual void AddtoInventory(GameObject item)
    {
        //현재 슬롯에 아이템이 있는지?
        //아이템을 얻는다
        //아이템 슬롯에 추가한다
        if (slots[currentItemSlot].isEmpty)
        {
            slots[currentItemSlot].isEmpty = false;
            slots[currentItemSlot].slotObj = item;
            item.GetComponent<Item>().PickUp(player);
        }
    }
    //현재 들고있는 물건을 버리는 함수
    public void RemovetoInventory()
    {
        if (slots[currentItemSlot].isEmpty == false)
        {
            slots[currentItemSlot].slotObj.GetComponent<Item>().PickDown(player);
            slots[currentItemSlot].isEmpty = true;
            slots[currentItemSlot].slotObj = null;
        }
    }
    public virtual void ChangeItemSlot(int index)
    {
        if (index < 0 || index > 3)
            return;
        GetCurrentItem()?.SetActive(false);
        currentItemSlot = index;
        GetCurrentItem()?.SetActive(true);
    }

    public virtual GameObject GetCurrentItem() => slots[currentItemSlot].slotObj;
    
}
