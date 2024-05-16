using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SteamPack_Item : Item, IItemUsable
{
    
    public override void UseItem()
    {
        if (this == null && Input.GetMouseButtonDown(0))
        {
        this.gameObject.SetActive(false);
        }
    }
}
    

