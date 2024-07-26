using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ShipController : NetworkBehaviour
{
    public Transform spawnPoint;
    [SerializeField]
    Transform leftDoor;
    [SerializeField]
    Transform rightDoor;

    [SerializeField]
    List<GameObject> players = new List<GameObject>();
    [SerializeField]
    List<GameObject> items = new List<GameObject>();

    #region OnTrigger Function
    private void OnTriggerEnter(Collider other)
    {
        OnServerSetParent(other.transform);
        if(other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            players.Add(player.gameObject);
        }
        else if(other.TryGetComponent<Item>(out Item item))
        {
            items.Add(item.gameObject);
            ItemReference.Instance.ExceptItemToList(item.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        OnServerUnsetParent(other.transform);
        if (players.Contains(other.gameObject))
        {
            players.Remove(other.gameObject);
        }
        else if (items.Contains(other.gameObject))
        {
            items.Remove(other.gameObject);
            ItemReference.Instance.AddItemToList(other.gameObject);
        }
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
                SetGameUI();
                yield break;
            }
            //transform.rotation = Quaternion.Slerp(transform.rotation, lookAt, 0.01f);
            OnServerChangePosition(Vector3.Slerp(transform.position, destination, 0.01f));
            yield return null;
        }
    }
    [Server]
    public void StartEscape(Vector3 destination)
    {
        // 대상 위치에서 현재 위치를 빼서 방향 벡터 계산
        Vector3 direction = destination - transform.position;

        direction.y = 0;
        if (direction != Vector3.zero)
            OnServerChangeRotation(Quaternion.LookRotation(direction));
        StartCoroutine(Escape(destination));
    }
    [Server]
    IEnumerator Escape(Vector3 destination)
    {
        while (true)
        {
            if (Vector3.Distance(transform.position, destination) < 0.1f)
            {
                OnServerChangePosition(destination);
                GameManager.Instance.OnServerSetActivePlayer(false);
                SetSelecterUI();
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
    [ClientRpc]
    public void SetGameUI()
    {
        UIController.Instance.SetActivateUI(typeof(UI_Setup));
    }
    [ClientRpc]
    public void SetSelecterUI()
    {
        UIController.Instance.SetActivateUI(typeof(UI_Selecter));
    }

    [Command(requiresAuthority = false)]
    public void OpenDoor()
    {
        //ShipController shipController = FindObjectOfType<ShipController>();
        //shipController.StartOpenCoroutine();
        StartOpenCoroutine();
    }
    [Command(requiresAuthority = false)]
    public void CloseDoor()
    {
        //ShipController shipController = FindObjectOfType<ShipController>();
        //shipController.StartCloseCoroutine();
        StartCloseCoroutine();
    }

    [ClientRpc]
    public void StartOpenCoroutine()
    {
        Debug.Log("CmdOpenDoor called");
        StartCoroutine(DoorOpenCoroutine());
    }
    [ClientRpc]
    public void StartCloseCoroutine()
    {
        StartCoroutine(DoorCloseCoroutine());
    }

    IEnumerator DoorOpenCoroutine()
    {
        while (true)
        {
            leftDoor.localScale = Vector3.Lerp(leftDoor.localScale, new Vector3(0, 1, 1), 0.1f);
            rightDoor.localScale = Vector3.Lerp(rightDoor.localScale, new Vector3(0, 1, 1), 0.1f);
            if (leftDoor.localScale.x < 0.01f && rightDoor.localScale.x < 0.01f)
            {
                leftDoor.localScale = new Vector3(0, 1, 1);
                rightDoor.localScale = new Vector3(0, 1, 1);
                break;
            }
            yield return null;
        }
    }
    IEnumerator DoorCloseCoroutine()
    {
        while (true)
        {
            leftDoor.localScale = Vector3.Lerp(leftDoor.localScale, new Vector3(1, 1, 1), 0.1f);
            rightDoor.localScale = Vector3.Lerp(rightDoor.localScale, new Vector3(1, 1, 1), 0.1f);
            if (leftDoor.localScale.x > 0.99f && rightDoor.localScale.x > 0.99f)
            {
                leftDoor.localScale = Vector3.one;
                rightDoor.localScale = Vector3.one;
                break;
            }
            yield return null;
        }
    }
}