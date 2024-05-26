using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IUIVisible, IInteractive
{
    private bool isOpen;
    private Animator animator;

    private void OnEnable()
    {
        animator = transform.parent.GetComponent<Animator>();
    }

    //플레이어에서 Ray를 쏴서 E를 누르면 GetComponent<Door>().UseDoor()하면 되지 않을까?
    public void OnInteractive()
    {
        UseDoor();
    }
    public void UseDoor()
    {
        if (!isOpen) { OpenDoor(); }
        else { CloseDoor(); }
    }

    public void OpenDoor()
    {
        isOpen = true;
        animator.SetBool("IsOpen", true);
    }

    public void CloseDoor() 
    { 
        isOpen = false;
        animator.SetBool("IsOpen", false);    
    }

    
}
