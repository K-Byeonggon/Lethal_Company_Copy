using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : NetworkBehaviour
{
    public List<Transform> loadedObjects;
    public List<Transform> playerObjects;

    //이전 위치
    private Vector3 previousPosition;
    private Quaternion previousRotation;

    private void Start()
    {
        previousPosition = transform.position;
        previousRotation = transform.rotation;
    }

    

    #region OnTrigger Function
    private void OnTriggerEnter(Collider other)
    {
        OnServerDependPlatform(other.transform);
    }
    private void OnTriggerExit(Collider other)
    {
        OnServerUndependPlatform(other.transform);
    }
    #endregion
    #region Server Function
    [Server] public void OnServerDependPlatform(Transform transform)
    {
        loadedObjects.Add(transform);
        if(transform.TryGetComponent<CharacterController>(out CharacterController cc))
        {
            playerObjects.Add(transform);
        }
    }
    [Server] public void OnServerUndependPlatform(Transform transform)
    {
        if(loadedObjects.Contains(transform))
            loadedObjects.Remove(transform);
        if(playerObjects.Contains(transform))
            playerObjects.Remove(transform);
    }
    [Server] public void OnServerChangePosition(Vector3 vec)
    {
        OnClientChangePosition(vec);
    }
    [Server] public void OnServerChangeRotation(Quaternion quaternion)
    {
        OnClientChangeRotation(quaternion);
    }
    #endregion
    #region ClientRpc Function
    [ClientRpc] public void OnClientChangePosition(Vector3 vec)
    {
        Vector3 moveVector = vec - previousPosition;
        transform.position = vec;
        loadedObjects.ForEach(transform => { transform.position += moveVector; });
        previousPosition = vec;
    }
    [ClientRpc] public void OnClientChangeRotation(Quaternion quaternion)
    {
        Quaternion currentRotation = quaternion;
        Quaternion rotationDelta = currentRotation * Quaternion.Inverse(previousRotation);

        foreach(Transform loaded in loadedObjects)
        {
            loaded.position = RotatePointAroundPivot(loaded.position, transform.position, rotationDelta);
            loaded.rotation = rotationDelta * loaded.rotation;
        }
        previousRotation = currentRotation;
    }
    #endregion

    // 특정 점을 피벗을 중심으로 회전시키는 함수
    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
    {
        return rotation * (point - pivot) + pivot;
    }
}
