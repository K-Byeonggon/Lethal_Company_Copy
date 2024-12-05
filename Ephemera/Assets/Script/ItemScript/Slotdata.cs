using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slotdata 
{
    public bool isEmpty;
    public Item slotObjComponent;
    public Slotdata()
    {
        isEmpty = true;
        slotObjComponent = null;
    }
}
