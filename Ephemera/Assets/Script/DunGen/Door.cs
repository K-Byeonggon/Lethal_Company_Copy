using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private bool isOpen;
    public bool IsOpen { get { return isOpen; } }
    [SerializeField] Animator animator;
    [SerializeField] UnityEngine.AI.NavMeshObstacle obstacle;

    //�÷��̾�� Ray�� ���� E�� ������ GetComponent<Door>().UseDoor()�ϸ� ���� ������?
    public void UseDoor()
    {
        if (!isOpen) { OpenDoor(); }
        else { CloseDoor(); }
    }

    public void OpenDoor()
    {
        isOpen = true;
        animator.SetBool("IsOpen", true);
        obstacle.enabled = true;
    }

    public void CloseDoor() 
    { 
        isOpen = false;
        animator.SetBool("IsOpen", false);
        obstacle.enabled = false;
    }

}
