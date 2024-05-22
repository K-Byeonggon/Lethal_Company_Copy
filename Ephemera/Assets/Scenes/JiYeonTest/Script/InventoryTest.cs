using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryTest : MonoBehaviour
{
    public static InventoryTest instance;
    public List<JItmeData> data = new List<JItmeData>(); 

    public void Awake()
    {
        instance = this;
    }

    public void Add(JItmeData itme)
    {
        data.Add(itme);
    }

    public void Remove(JItmeData itme)
    {
        data.Remove(itme);
    }
}
