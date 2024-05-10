using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemObtainable
{
    public void ShowPickupUI();
    public void PickUp();
    public void PickDown();
}
