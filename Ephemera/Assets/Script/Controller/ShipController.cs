using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TreeEditor;
using UnityEngine;

public class ShipController : NetworkBehaviour
{
    public Transform spawnPoint;

    #region OnTrigger Function
    private void OnTriggerEnter(Collider other)
    {
        OnServerSetParent(other.transform);
    }
    private void OnTriggerExit(Collider other)
    {
        OnServerUnsetParent(other.transform);
    }
    #endregion
    #region Server Function
    [Server]
    public void StartLanding(Vector3 destination)
    {
        // 대상 위치에서 현재 위치를 빼서 방향 벡터 계산
        Vector3 direction = destination - transform.position;

        direction.y = 0;
        if (direction != Vector3.zero)
            OnServerChangeRotation(Quaternion.LookRotation(direction));
        StartCoroutine(Landing(destination));
    }

    [Server]
    IEnumerator Landing(Vector3 destination)
    {
        while (true)
        {
            if (Vector3.Distance(transform.position, destination) < 0.1f)
            {
                OnServerChangePosition(destination);
                GameManager.Instance.OnServerActiveLocalPlayerCamera();
                GameManager.Instance.OnServerSetActivePlayer(true);
                UIController.Instance.SetActivateUI(typeof(UI_Setup));
                yield break;
            }
            //transform.rotation = Quaternion.Slerp(transform.rotation, lookAt, 0.01f);
            OnServerChangePosition(Vector3.Slerp(transform.position, destination, 0.01f));
            yield return null;
        }
    }
    [Server]
    public void OnServerChangePosition(Vector3 vec)
    {
        transform.position = vec;
    }
    [Server]
    public void OnServerChangeRotation(Quaternion quaternion)
    {
        transform.rotation = quaternion;
    }
    [Server]
    public void OnServerSetParent(Transform player)
    {
        OnClientSetParent(player);
    }
    [Server]
    public void OnServerUnsetParent(Transform player)
    {
        OnClientUnsetParent(player);
    }
    #endregion
    #region ClientRpc Function
    [ClientRpc]
    public void OnClientSetParent(Transform player)
    {
        player.parent = this.gameObject.transform;
    }
    [ClientRpc]
    public void OnClientUnsetParent(Transform player)
    {
        player.parent = null;
    }
    #endregion
}