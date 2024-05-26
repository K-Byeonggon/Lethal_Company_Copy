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
        //Debug.Log("�΋H�����");
        thumper.hitWall = true;
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("hitWall �����ؿ�");
        thumper.hitWall = false;
    }
}
