using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptble Object/Item Data")]
public class ItmeData : ScriptableObject
{
    [Header("ITEM DATA")]
    public int itemid;
    public string itemName;
    public int amount;

    [Header("#WEAPON")]
    public GameObject prefab;
}
