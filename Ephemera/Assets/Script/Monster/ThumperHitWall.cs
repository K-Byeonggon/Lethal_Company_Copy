using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumperHitWall : MonoBehaviour
{
    ThumperAI thumper;
    // Start is called before the first frame update
    void Start()
    {
        thumper = GetComponent<ThumperAI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ºÎ‹HÇû¾î¿ä");
        thumper.hitWall = true;
    }

    private void OnTriggerExit(Collider other)
    {
        thumper.hitWall = false;
    }
}
