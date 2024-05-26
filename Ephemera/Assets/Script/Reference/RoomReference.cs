using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomReference : MonoBehaviour
{
    public static RoomReference Instance;

    [SerializeField]
    List<RoomRegister> roomList = new List<RoomRegister>();

    public int RoomCount => roomList.Count;

    private void Awake()
    {
        Instance = this;
    }

    public void AddRoomToDic(RoomRegister room)
    {
        Instance.roomList.Add(room);
    }

    public Vector3 GetRandomPosition()
    {
        int roomIndex = Random.Range(0, Instance.roomList.Count);
        int positionIndex = Random.Range(0, Instance.roomList[roomIndex].ItemSpawnPosition.Count);
        return Instance.roomList[roomIndex].ItemSpawnPosition[positionIndex].transform.position;
    }
    public void ClearRoom()
    {
        roomList.Clear();
    }
}
