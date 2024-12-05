using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buzzer_Item : Item
{
    [SerializeField]
    private AudioClip audioClip;
    [SerializeField]
    private AudioSource audioSource;

    [ClientRpc]
    public override void UseItem()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
            Debug.Log("˜—˜—!");
        }
    }
}
