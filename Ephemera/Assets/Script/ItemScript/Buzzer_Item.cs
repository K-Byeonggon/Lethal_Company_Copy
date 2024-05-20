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

        // Assuming the player has a tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInventory = player.GetComponent<Inventory>();
            if (playerInventory == null)
            {
                Debug.LogError("Inventory component not found on the Player object.");
            }
        }
        else
        {
            Debug.LogError("Player object with tag 'Player' not found.");
        }

        // Ensure the item has the "UsableItem" tag
        gameObject.tag = "UsableItem";
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
        if (audioClip != null && playerInventory != null && playerInventory.IsUsable(gameObject))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
                Debug.Log("˜—˜—!");
            }
        }
        else
        {
            Debug.Log("Item is not usable or inventory is null.");
        }
    }
}
