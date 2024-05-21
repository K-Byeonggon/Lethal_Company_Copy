using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCharacterCustom : NetworkBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (!isOwned)
            return;
        if(Input.GetKeyDown(KeyCode.A))
        {
            transform.position = transform.position + Vector3.left;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.position = transform.position + Vector3.right;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.position = transform.position + Vector3.forward;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.position = transform.position + Vector3.back;
        }
    }
}
