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

    //�÷��̾�� Ray�� ���� E�� ������ GetComponent<Door>().UseDoor()�ϸ� ���� ������?
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
