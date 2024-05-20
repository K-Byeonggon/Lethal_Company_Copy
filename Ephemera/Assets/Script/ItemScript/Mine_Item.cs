using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine_Item : MonoBehaviour
{
    [SerializeField]
    private GameObject explosionPrefab; 

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log(collision.gameObject.name);
            ActiveMine();
        }
    }
    public void ActiveMine()
    {
        Debug.Log("Æã");

        GameObject explosionObject =  PoolManager.Instance.GetGameObject(explosionPrefab);
        explosionObject.transform.position = this.transform.position;
        PoolManager.Instance.ReturnToPool(this.gameObject);
    }
}
