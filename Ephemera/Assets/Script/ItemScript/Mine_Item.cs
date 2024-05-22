using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine_Item : MonoBehaviour
{
    [SerializeField]
    private GameObject explosionPrefab;
    [SerializeField]
    private AudioSource explosionSound;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log(other.gameObject.name);
            ActiveMine();
            
        }
    }
    
    public void ActiveMine()
    {
        Debug.Log("Æã");

        GameObject explosionObject =  PoolManager.Instance.GetGameObject(explosionPrefab);
        explosionObject.transform.position = this.transform.position;
        PoolManager.Instance.ReturnToPool(this.gameObject);
        explosionSound.Play();
        Destroy(this.gameObject);
    }

}
