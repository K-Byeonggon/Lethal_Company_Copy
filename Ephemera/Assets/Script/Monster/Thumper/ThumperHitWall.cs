using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumperHitWall : MonoBehaviour
{
    ThumperAI thumper;
    // Start is called before the first frame update
    void Start()
    {
        thumper = transform.parent.GetComponent<ThumperAI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("부딫혔어요");
        thumper.hitWall = true;
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("hitWall 갱신해용");
        thumper.hitWall = false;
    }
}
