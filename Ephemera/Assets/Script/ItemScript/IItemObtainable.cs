using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemObtainable
{
    public int ItemPrice { get; }
    public void PickUp(Transform pickTransform);
    public void PickDown(Transform pickTransform);
}
