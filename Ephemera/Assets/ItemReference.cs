using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemReference : MonoBehaviour
{
    public static ItemReference Instance;

    [SerializeField]
    public List<GameObject> itemList = new List<GameObject>();

    public int ItemCount => itemList.Count;

    private void Awake()
    {
        Instance = this;
    }

    public void AddItemToList(GameObject item)
    {
        Instance.itemList.Add(item);
    }
    public void DestroyItem(GameObject item)
    {
        if (itemList.Contains(item))
        {
            itemList.Remove(item);
            NetworkIdentity identity = item.GetComponent<NetworkIdentity>();
            GameManager.Instance.DestroyObject(identity);
        }
    }
    public void DestroyAll()
    {
        for (int i = itemList.Count - 1; i >= 0; i--)
        {
            GameObject item = itemList[i];

            if (item != null)
            {
                NetworkIdentity identity = item.GetComponent<NetworkIdentity>();
                GameManager.Instance.DestroyObject(identity);
            }
            itemList.RemoveAt(i);
        }
    }
}
