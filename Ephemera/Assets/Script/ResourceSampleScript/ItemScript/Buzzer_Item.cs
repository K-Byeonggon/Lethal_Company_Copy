using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buzzer_Item : Item, IItemUsable
{
    public AudioClip audioClip; 
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = audioClip;
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UseItem();
        }
    }

    public override void UseItem()
    {
        if (audioClip != null)
        {
           
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
                Debug.Log("˜—˜—!");
            }
        }
    }
}
