using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnareFleaDetect : MonoBehaviour
{
    SnareFleaAI snare;

    private void Start()
    {
        snare = transform.parent.GetComponent<SnareFleaAI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            snare.sawPlayer = true;
            snare.player = other.transform;
        }
        
    }
    /*
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            snare.sawPlayer = false;
        }
    }*/
}
