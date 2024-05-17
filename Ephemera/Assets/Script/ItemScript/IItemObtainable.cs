using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemObtainable
{
    public void ShowPickupUI();
    public void PickUp(PlayerEx owner);
    public void PickDown(PlayerEx owner);
    public int SellItem();
}
