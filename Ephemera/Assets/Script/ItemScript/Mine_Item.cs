using Mirror;
using System.Collections;
using UnityEngine;

public class Mine_Item : NetworkBehaviour
{
    [SerializeField]
    private GameObject explosionPrefab;
    [SerializeField]
    private AudioSource explosionSound;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("ObtainableItem"))
        {
            explosionSound.Play();
            Debug.Log(other.gameObject.name);
            ActiveMine();
        }
    }

    public void ActiveMine()
    {
        Debug.Log("Æã");
        
        GameObject explosionObject = PoolManager.Instance.GetGameObject(explosionPrefab);
        explosionObject.transform.position = this.transform.position;
        PoolManager.Instance.ReturnToPool(this.gameObject);

        
    }

   
}
