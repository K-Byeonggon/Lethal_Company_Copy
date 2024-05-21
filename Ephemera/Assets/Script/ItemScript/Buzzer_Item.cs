using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buzzer_Item : Item, IItemUsable
{
    public AudioClip audioClip;
    private AudioSource audioSource;
    private Inventory playerInventory;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = audioClip;
    }

    public override void UseItem()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
            Debug.Log("˜—˜—!");
        }
    }
}
