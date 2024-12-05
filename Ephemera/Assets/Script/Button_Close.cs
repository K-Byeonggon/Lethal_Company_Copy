using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Close : NetworkBehaviour, IInteractive, IUIVisible
{
    [SerializeField]
    ShipController shipController;
    public void OnInteractive()
    {
        shipController.CloseDoor();
    }
}
