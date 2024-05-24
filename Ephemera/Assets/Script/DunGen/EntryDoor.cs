using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryDoor : MonoBehaviour
{
    public Transform exitPosition;
    public void UseDoor(GameObject player)
    {
        player.transform.position = exitPosition.position;
    }
}
