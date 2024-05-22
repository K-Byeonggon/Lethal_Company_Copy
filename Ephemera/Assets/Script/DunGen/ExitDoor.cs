using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    public Transform entryPosition;
    public void UseDoor(GameObject player)
    {
        player.transform.position = entryPosition.position;
    }
}
