using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField] MonsterAI monster;
    [SerializeField] UnityEngine.AI.NavMeshAgent navMeshAgent;

    private bool _opening = false;

    
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Door"))
        {
            Door door = other.GetComponent<Door>();
            if(door.IsOpen) { return; }

            if (!_opening)
            {
                _opening = true;
                StartCoroutine(OpenCoroutine(door));
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            Door door = other.GetComponent<Door>();

            if (door.IsOpen)
            {
                _opening = false;
            }
        }
    }

    
    private IEnumerator OpenCoroutine(Door door)
    {
        navMeshAgent.isStopped = true;
        yield return new WaitForSeconds(monster.OpenDoorDelay);
        door.OpenDoor();
        yield return new WaitForSeconds(0.6f);//문열리는 시간
        navMeshAgent.isStopped = false;
    }
    /*
    private IEnumerator OpenCoroutine(Door door)
    {
        navMeshAgent.isStopped = true;
        yield return new WaitForSeconds(3f);
        door.OpenDoor();
        navMeshAgent.isStopped = false;
    }*/
}
