using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemSell : MonoBehaviour
{
    public JItmeData Item;

    public void Sell()
    {
        InventoryTest.instance.Add(Item);
    }
}
