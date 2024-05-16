using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowBar_Item : Item,IItemUsable
{
    // Start is called before the first frame update
    public override void UseItem()
    {
        if (this != null &&Input.GetMouseButtonDown(0))
        {
            
        }
    }
}
