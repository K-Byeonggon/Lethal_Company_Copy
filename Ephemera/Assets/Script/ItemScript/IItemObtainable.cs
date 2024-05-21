using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemObtainable
{
    public void PickUp(Inventory owner);
    public void PickDown(Inventory owner);
    public int SellItem();
}
