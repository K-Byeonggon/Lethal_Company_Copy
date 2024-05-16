using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnPoint : MonoBehaviour
{
    [SerializeField] public GameObject[] item;
    public GameObject spawnItem;
    private Vector3 spawnPoint;
    private void Start()
    {
        SpawnItem();
    }

    private void SpawnItem()
    {
        spawnPoint = this.transform.position;
        GameObject selectedItem = item[Random.Range(0,item.Length)];
        spawnItem = Instantiate(selectedItem, spawnPoint,Quaternion.identity);
    }
}
