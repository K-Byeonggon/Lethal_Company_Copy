using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomRegister : MonoBehaviour
{
    public List<Transform> ItemSpawnPosition = new List<Transform>();
    void Start()
    {
        if(RoomReference.Instance != null)
            RoomReference.Instance.AddRoomToDic(this);
    }
}