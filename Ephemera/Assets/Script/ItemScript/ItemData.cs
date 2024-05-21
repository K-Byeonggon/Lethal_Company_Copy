using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public string itemName;
    public int itemMinPrice;
    public int itemMaxPrice;
    public bool isBothHand;
    public Sprite image;
    public int GetRandomPrice()
    {
        return Random.Range(itemMinPrice, itemMaxPrice);
    }
}
