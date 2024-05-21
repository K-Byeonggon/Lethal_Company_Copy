using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slotdata 
{
    public bool isEmpty;
    public GameObject slotObj;
    public Item slotObjComponent;
    public Slotdata()
    {
        isEmpty = true;
        slotObj = null;
        slotObjComponent = null;
    }
}
