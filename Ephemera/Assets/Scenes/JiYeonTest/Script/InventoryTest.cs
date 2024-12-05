using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryTest : MonoBehaviour
{
    public static InventoryTest instance;
    public List<ItemData> data = new List<ItemData>(); 

    public void Awake()
    {
        instance = this;
    }

    public void Add(ItemData item)
    {

    }

    public void Remove(ItemData itme)
    {

    }
}
