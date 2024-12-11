using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnareFleaDetect : MonoBehaviour
{
    //SnareFleaAI snare;
    SnareFleaStateAI snare;

    private void Start()
    {
        snare = transform.parent.GetComponent<SnareFleaStateAI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            snare.hasSeenPlayer = true;
            snare.player = other.transform;
            snare.playersHead = snare.player.GetChild(2);
            Debug.Log($"올무벌레가 플레이어{other.name} 탐지.");
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
