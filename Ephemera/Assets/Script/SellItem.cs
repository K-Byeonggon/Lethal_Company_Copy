using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellItem : NetworkBehaviour, IUIVisible
{
    [Command(requiresAuthority = false)]
    public void OnInteractive(int value)
    {
        GameManager.Instance.CurrentMoney += value;
    }
}
