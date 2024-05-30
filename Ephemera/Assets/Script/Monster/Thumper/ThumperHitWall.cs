using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumperHitWall : NetworkBehaviour
{
    [SerializeField]
    ThumperAI thumper;
    
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("부딫혔어요");
        thumper.hitWall = true;
    }

    [ServerCallback]
    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("hitWall 갱신해용");
        thumper.hitWall = false;
    }
}
