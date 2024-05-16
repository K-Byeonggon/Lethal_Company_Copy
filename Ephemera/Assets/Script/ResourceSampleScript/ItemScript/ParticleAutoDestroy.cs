using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoDestroy : MonoBehaviour
{
    [SerializeField]
    private float destroyTime = 3.0f;
    
    private float currentTime = 0;
    void FixedUpdate()
    {
        if(currentTime > destroyTime)
        {
            PoolManager.Instance.ReturnToPool(this.gameObject);
        }
        currentTime += Time.fixedDeltaTime;
    }
}
